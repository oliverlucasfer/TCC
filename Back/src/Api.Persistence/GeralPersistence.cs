using System.Threading.Tasks;
using Api.Persistence.Contexto;
using Api.Persistence.Contratos;

namespace Api.Persistence
{
    public class GeralPersistence : IGeralPersistence
    {
        private readonly ApiContext _context;
        public GeralPersistence(ApiContext context)
        {
            _context = context;

        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

    }
}