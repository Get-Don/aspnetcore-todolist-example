using Microsoft.EntityFrameworkCore;
using RepositoryBase;
using RepositoryEfCore.Data;

namespace RepositoryEfCore;

public class Repository<T> : IRepository<T> where T: class
{
    private readonly ApplicationDbContext _db;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _db.SaveChangesAsync();

        return entity;
    }

    public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task RemoveAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAll()
    {
        await _dbSet.ExecuteDeleteAsync();
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        // Note: 존재하지 않는 데이터를 업데이트 할 때 예외가 발생하지 않는다.
        // Insert가 되거나 아무일도 발생하지 않는다.
        // 이점을 주의해야 한다.
        _dbSet.Update(entity);
        await _db.SaveChangesAsync();
    }
}
