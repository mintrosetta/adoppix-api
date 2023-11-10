namespace AdopPix.Services.IServices
{
    public interface ITagService
    {
        Task<string> FindByIdAsync(string tagId);
    }
}