using System;
using NUnit.Framework;
using Project.Domain.Memos.Model;

namespace Test.Domain {

    /// <summary>
    /// Tag の同一性 (Equals / GetHashCode) に関するテスト．
    /// </summary>
    public sealed class Test_TagEquality {

        [Test]
        public void SameId_AreEqual() {
            var id = new TagId(Guid.NewGuid());
            var a = new Tag(id, "Work");
            var b = new Tag(id, "Work");

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a == b);
        }

        [Test]
        public void DifferentId_AreNotEqual() {
            var a = new Tag(new TagId(Guid.NewGuid()), "Work");
            var b = new Tag(new TagId(Guid.NewGuid()), "Work");

            Assert.IsFalse(a.Equals(b));
        }

        // Equals/GetHashCode の規約: 等しいオブジェクトは同じハッシュ値を返さなければならない．
        // Tag の同一性は Id で判定されるため，ハッシュ値も Id 由来である必要がある．
        // (この観点を Name 由来の GetHashCode が壊していた)
        [Test]
        public void EqualTags_ById_ShareHashCode() {
            var id = new TagId(Guid.NewGuid());
            var a = new Tag(id, "Work");
            var b = new Tag(id, "Home"); // 名前は異なるが Id が同じ => 同一とみなされる

            Assert.IsTrue(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
