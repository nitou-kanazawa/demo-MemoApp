using System;
using System.IO;
using System.Linq;
using SQLite;
using Project.Domain.Memos.Model;
using Project.Domain.Memos.Repository;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

// [REF]
//  _: sqlite-net-pclで新規のテーブルを作成する方法 https://genesis-tech.jp/blog/how-to-create-new-table-sqlitenetpcl/

namespace Project.Infrastructure.SQLiteNet.Memos {

    public sealed class SQLiteMemoRepository : IMemoRepository {

        private readonly ISQLiteConnection _connection;

        private readonly static string FileName = "memos.db";
        private readonly static string FilePath = Path.Combine(UnityEngine.Application.persistentDataPath, FileName);


        /// ----------------------------------------------------------------------------
        // Public Method

        /// <summary>
        /// コンストラクタ．
        /// </summary>
        public SQLiteMemoRepository() {
            _connection = new SQLiteConnection(FilePath);
            _connection.CreateTable<MemoRecord>();
        }

        /// <summary>
        /// 終了処理．
        /// </summary>
        public void Dispose() {
            _connection.Dispose();
        }


        /// ----------------------------------------------------------------------------
        // Public Method (Interface)

        /// <summary>
        /// 指定したメモを保存する．
        /// </summary>
        UniTask IMemoRepository.SaveAsync(Memo memo) {
            if (memo == null) throw new ArgumentNullException(nameof(memo));

            var record = memo.ToRecord();

            // Apply to database
            if (_connection.Find<MemoRecord>(record.Id) != null) {
                _connection.Update(record);
            } else {
                _connection.Insert(record);
            }

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 指定したIDのメモを検索する．
        /// </summary>
        UniTask<Memo> IMemoRepository.FindByIdAsync(Guid id) {
            var record = _connection.Find<MemoRecord>(id);
            var memo = record?.ToDomain();

            return UniTask.FromResult(memo);
        }

        /// <summary>
        /// 全てのメモを取得する．
        /// </summary>
        UniTask<IEnumerable<Memo>> IMemoRepository.GetAllAsync() {
            var records = _connection.Table<MemoRecord>().ToList();
            var memos = records.Select(r => r.ToDomain());

            return UniTask.FromResult(memos);
        }

        /// <summary>
        /// 指定したIDのメモを削除する．
        /// </summary>
        UniTask IMemoRepository.DeleteAsync(Guid id) {
            _connection.Delete<MemoRecord>(id);

            return UniTask.CompletedTask;
        }
    }
}

