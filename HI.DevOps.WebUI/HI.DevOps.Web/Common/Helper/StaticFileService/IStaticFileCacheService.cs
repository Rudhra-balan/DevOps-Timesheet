namespace HI.DevOps.Web.Common.Helper.StaticFileService
{
    public interface IStaticFileCacheService
    {
        string GetFilePath(string relativePath);
    }
}