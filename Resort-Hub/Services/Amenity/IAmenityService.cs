namespace Resort_Hub.Services.Amenity
{
    public interface IAmenityService
    {
        void Add(Entities.Amenity amenity);
        Entities.Amenity? GetById(int id);
        List<Entities.Amenity> GetAll();
        void Update(Entities.Amenity amenity);
        void Delete(Entities.Amenity amenity);
        Task SaveChangesAsync();

    }
}
