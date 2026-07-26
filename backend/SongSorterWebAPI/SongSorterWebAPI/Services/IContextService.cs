namespace SongSorterWebAPI.Services
{
    public interface IContextService
    {
        public Task<int> ContextSaveChangesAsync();
  
    }
}
