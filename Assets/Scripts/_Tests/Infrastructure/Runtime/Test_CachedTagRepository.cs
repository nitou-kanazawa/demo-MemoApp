using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine.TestTools;
using Cysharp.Threading.Tasks;
using Project.Domain.Memos.Model;
using Project.Domain.Memos.Repository;
using Project.Infrastructure.BuiltIn.Memos;

namespace Tests.Infrastructure {

    /// <summary>
    /// CachedTagRepository のキャッシュ整合性に関するテスト．
    /// inner には実物の InMemoryTagRepository を用いる．
    /// </summary>
    public sealed class Test_CachedTagRepository {

        // 回帰テスト:
        // AddAsync がキャッシュに1件入れた後でも，GetAllAsync は
        // (キャッシュ済みの部分集合ではなく) inner の全件を返さなければならない．
        [UnityTest]
        public IEnumerator GetAllAsync_AfterAdd_ReturnsAllTags() => UniTask.ToCoroutine(async () => {
            ITagRepository inner = new InMemoryTagRepository();
            await inner.AddAsync(new Tag("A"));
            await inner.AddAsync(new Tag("B"));
            await inner.AddAsync(new Tag("C"));

            ITagRepository sut = new CachedTagRepository(inner);

            // キャッシュに1件だけ入った状態を作る (バグの誘発条件)．
            await sut.AddAsync(new Tag("D"));

            var all = (await sut.GetAllAsync()).ToList();

            // 修正前はキャッシュ済みの1件しか返らなかった．
            Assert.AreEqual(4, all.Count);
        });

        [UnityTest]
        public IEnumerator GetAllAsync_LoadsAllFromInner_WhenCacheEmpty() => UniTask.ToCoroutine(async () => {
            ITagRepository inner = new InMemoryTagRepository();
            await inner.AddAsync(new Tag("A"));
            await inner.AddAsync(new Tag("B"));

            ITagRepository sut = new CachedTagRepository(inner);

            var all = (await sut.GetAllAsync()).ToList();

            Assert.AreEqual(2, all.Count);
        });
    }
}
