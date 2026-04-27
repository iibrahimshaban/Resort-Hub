using Microsoft.Extensions.Options;
using Resort_Hub.Configuration;
using Resort_Hub.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Resort_Hub.Services.Amenity
{
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Add(Entities.Amenity amenity)
        {
            _unitOfWork.Amenities.Add(amenity);
        }

        public Entities.Amenity? GetById(int id)
        {
           return _unitOfWork.Amenities.GetById(id);
        }

        public List<Entities.Amenity> GetAll()
        {
            return _unitOfWork.Amenities.GetAll();
        }
        
        public async Task SaveChangesAsync()
        {
            await _unitOfWork.SaveAsync();
        }

        public void Update(Entities.Amenity amenity)
        {
            _unitOfWork.Amenities.Update(amenity);
        }

        public void Delete(Entities.Amenity amenity)
        {
            _unitOfWork.Amenities.Delete(amenity);
        }
    }
}
