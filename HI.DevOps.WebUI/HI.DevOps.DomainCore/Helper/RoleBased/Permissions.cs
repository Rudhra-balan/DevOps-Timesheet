using System.Collections.Generic;
using System.Security.Claims;
using HI.DevOps.DomainCore.Enumeration;

namespace HI.DevOps.DomainCore.Helper.RoleBased
{
    public class Permissions
    {
        // define basic policies
        public const string ReaderPolicy = "ReaderPolicy";
        public const string UserPolicy = "UserPolicy";
        public const string AdminPolicy = "AdminPolicy";

        public const string ReaderRole = "reader";
        public const string UserRole = "user";
        public const string AdminRole = "admin";
        private static Permissions _instance;

        // define basic policy claims
        public static string[] ReaderPolicyClaims = {PermissionsEnum.LeastPriviligedAccess};

        public static string[] AdminPolicyClaims =
        {
            PermissionsEnum.Section4, PermissionsEnum.Section5,
            PermissionsEnum.Section1, PermissionsEnum.Section3,
            PermissionsEnum.Section2, PermissionsEnum.LeastPriviligedAccess
        };

        public static string[] UserPolicyClaims =
        {
            PermissionsEnum.Section1, PermissionsEnum.Section3,
            PermissionsEnum.Section2, PermissionsEnum.LeastPriviligedAccess
        };

        // private variables to hold working data
        //
        private readonly ClaimsPrincipal _user;


        /// <summary>
        ///     constructor initialized with user
        /// </summary>
        /// <param name="user"></param>
        public Permissions(ClaimsPrincipal user)
        {
            _user = user;
        }

        /// <summary>
        ///     constructor without user
        /// </summary>
        public Permissions()
        {
            _user = null;
        }

        /// <summary>
        ///     expose a static instance of class
        /// </summary>
        public static Permissions Instance
        {
            get
            {
                if (_instance == null) _instance = new Permissions();
                return _instance;
            }
        }

        /// <summary>
        ///     checks if user can upload files
        /// </summary>
        public bool CanUploadFiles => _user.IsInRole("Standard") || _user.IsInRole("Admin");

        /// <summary>
        ///     checks if user can delete data
        /// </summary>
        public bool CanDeleteItems => _user.IsInRole("Admin");


        /// <summary>
        ///     checks if user has a specific claim.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermission(ClaimsPrincipal user, string permission)
        {
            var ret = user.HasClaim(v => v.Type == permission);

            return ret;
        }

        /// <summary>
        ///     Method sets default claims based on the system supported roles.
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="role"></param>
        public void SetDefaultClaimsByRole(List<Claim> claims, string role)
        {
            switch (role.ToLower())
            {
                case "admin":
                    claims.Add(new Claim(PermissionsEnum.Section5, PermissionsEnum.Section5));
                    claims.Add(new Claim(PermissionsEnum.Section4, PermissionsEnum.Section4));
                    claims.Add(new Claim(PermissionsEnum.Section1, PermissionsEnum.Section1));
                    claims.Add(new Claim(PermissionsEnum.Section3, PermissionsEnum.Section3));
                    claims.Add(new Claim(PermissionsEnum.Section2, PermissionsEnum.Section2));
                    claims.Add(new Claim(PermissionsEnum.LeastPriviligedAccess,
                        PermissionsEnum.LeastPriviligedAccess));
                    break;
                case "user":
                    claims.Add(new Claim(PermissionsEnum.Section1, PermissionsEnum.Section1));
                    claims.Add(new Claim(PermissionsEnum.Section3, PermissionsEnum.Section3));
                    claims.Add(new Claim(PermissionsEnum.Section2, PermissionsEnum.Section2));
                    claims.Add(new Claim(PermissionsEnum.LeastPriviligedAccess,
                        PermissionsEnum.LeastPriviligedAccess));
                    break;
                case "reader":
                    claims.Add(new Claim(PermissionsEnum.LeastPriviligedAccess,
                        PermissionsEnum.LeastPriviligedAccess));
                    break;
            }
        }
    }
}