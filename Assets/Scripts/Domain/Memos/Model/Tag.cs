using System;
using Project.Domain.Shared;

namespace Project.Domain.Memos.Model {

    /// <summary>
    /// タグを表すEntity．
    /// </summary>
    public sealed class Tag : EntityBase<TagId> {

        public string Name { get; }


        /// ----------------------------------------------------------------------------
        // Public Method

        /// <summary>
        /// コンストラクタ．
        /// </summary>
        public Tag(TagId id, string name)
            : base(id) {

            if (string.IsNullOrEmpty(name)) {
                throw new InvalidOperationException();
            }
            Name = name;
        }

        /// <summary>
        /// コンストラクタ．
        /// </summary>
        public Tag(string name) : this(new TagId(), name) { }


        /// <summary>
        /// 文字列への変換．
        /// </summary>
        public override string ToString() {
            return Name;
        }

        /// <summary>
        /// 複製．
        /// </summary>
        public Tag Clone() {
            return new Tag(Id, Name);
        }
    }
}
