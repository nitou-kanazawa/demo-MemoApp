using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Cysharp.Threading.Tasks;
using Project.Domain.Memos.Model;
using Project.Domain.Memos.Repository;

namespace Project.Infrastructure.BuiltIn.Memos {

    /// <summary>
    /// <see cref="ITagRepository"/>にキャッシュを導入して処理の効率化を図るラッパー（プロキシ）．
    /// </summary>
    public sealed class CachedTagRepository : ITagRepository {
        
        private readonly ITagRepository _innerRepository;
        private readonly ConcurrentDictionary<TagId, Tag> _cache = new();
        private bool _isFullyLoaded = false;


        /// ----------------------------------------------------------------------------
        // Public Method

        public CachedTagRepository(ITagRepository innerRepository) {
            _innerRepository = innerRepository;
        }

        public void Dispose() {
            _cache.Clear();
            _innerRepository.Dispose();
        }


        /// ----------------------------------------------------------------------------
        // Public Method (Interface)

        async UniTask ITagRepository.AddAsync(Tag tag) {
            await _innerRepository.AddAsync(tag);
            _cache[tag.Id] = tag;
        }

        async UniTask<IEnumerable<Tag>> ITagRepository.GetAllAsync() {
            if (!_isFullyLoaded) {
                var tags = await _innerRepository.GetAllAsync();
                _cache.Clear();
                foreach (var tag in tags) {
                    _cache[tag.Id] = tag;
                }
                _isFullyLoaded = true;
            }
            return _cache.Values;
        }

        async UniTask<Tag> ITagRepository.FindByIdAsync(TagId id) {
            if (_cache.TryGetValue(id, out var tag))
                return tag;
            return await _innerRepository.FindByIdAsync(id);
        }

        async UniTask<Tag> ITagRepository.FindByNameAsync(string name) {
            var tag = _cache.Values.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (tag != null)
                return tag;
            return await _innerRepository.FindByNameAsync(name);
        }

        async UniTask ITagRepository.RemoveAsync(TagId id) {
            await _innerRepository.RemoveAsync(id);
            _cache.TryRemove(id, out _);
        }
    }

}
