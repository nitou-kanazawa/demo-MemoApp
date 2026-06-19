using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using Project.Domain.Memos.Model;
using Project.Domain.Memos.Repository;
using Project.Application.UseCase;
using Project.Infrastructure.BuiltIn.Memos;
using Project.Infrastructure.SQLiteNet.Memos;

namespace Project.Composition {
    
    public sealed class RootLifetimeScope : LifetimeScope {
        
        protected override void Configure(IContainerBuilder builder) {

            // Repository
            builder.Register<SQLiteTagRepository>(Lifetime.Singleton).As<ITagRepository>();

            // Entry
            builder.RegisterEntryPoint<StartUp>();

        }
    
    }


    public sealed class StartUp : IAsyncStartable {

        private readonly ITagRepository _repository;
        private readonly TagUseCase _tagUseCase;

        public StartUp(ITagRepository tagRepository) {
            _repository = tagRepository;
            _tagUseCase = new TagUseCase(_repository);
        }

        public async UniTask StartAsync(CancellationToken cancellation = default) {

            var tagNames = new string[] {
                "ExampleTag",
                "Home",
                "Anyway",
                "Uniqu",
                "Uniqu",
            };

            // タグの作成
            foreach(var name in tagNames) {
                if (await _tagUseCase.CanAddTagAsync(name)) {
                    await _tagUseCase.CreateTagAsync(name);
                }
            }


            // タグの検索 (名前で)
            var fetchedTag = await _repository.FindByNameAsync("ExampleTag");
            Debug.Log($"Fetched tag: {fetchedTag?.Name}");

            // タグの削除
            if (fetchedTag != null) {
                await _repository.RemoveAsync(fetchedTag.Id);
                Debug.Log($"Tag removed: {fetchedTag.Name}");
            }

            // 全てのタグを取得
            var allTags = (await _repository.GetAllAsync()).ToList();
            Debug.Log($"Total tags: {allTags.Count()}");
        }
    }
}
