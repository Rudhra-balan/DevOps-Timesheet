using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;

namespace HI.DevOps.Web.Common.Helper.StaticFileService
{
    public class StaticFileCacheService : IStaticFileCacheService
    {
        #region Constructors

        /// <summary>
        ///     Create a new repository
        /// </summary>
        public StaticFileCacheService(IMemoryCache cache, IWebHostEnvironment environment)
        {
            _cache = cache;
            _environment = environment;
            _fileProvider = environment.WebRootFileProvider;
        }

        #endregion

        #region Get methods

        /// <summary>
        ///     Get a file path with a version hash
        /// </summary>
        public string GetFilePath(string relativePath)
        {
            // Get the hash
            if (_cache.TryGetValue(relativePath, out string hash)) return $"{relativePath}?v={hash}";
            // Create an absolute path
            var absolutePath = _environment.WebRootPath + relativePath;

            // Make sure that the file exists
            if (File.Exists(absolutePath) == false) return relativePath;

            // Create cache options
            var cacheEntryOptions = new MemoryCacheEntryOptions();

            // Add an expiration token that watches for changes in a file
            cacheEntryOptions.AddExpirationToken(_fileProvider.Watch(relativePath));

            // Create a hash of the file
            using var md5 = MD5.Create();
            using Stream stream = File.OpenRead(absolutePath);
            hash = Convert.ToBase64String(md5.ComputeHash(stream));

            // Insert the hash to cache
            _cache.Set(relativePath, hash, cacheEntryOptions);

            // Return the url
            return $"{relativePath}?v={hash}";
        } // End of the GetFilePath method

        #endregion

        #region Variables

        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _environment;
        private readonly IFileProvider _fileProvider;

        #endregion
    }
}