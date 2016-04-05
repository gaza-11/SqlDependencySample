using System;
using System.Data.SqlClient;
using System.Threading;

namespace SqlDependencySample
{
    /// <summary>
    /// SqlDependencyサンプルクラス
    /// </summary>
    internal class DependencySample
    {
        /// <summary>
        /// ロック用オブジェクト
        /// </summary>
        private static object lockObj = new object();

        /// <summary>
        /// 接続文字列
        /// </summary>
        private string ConnectionString { get; set; }

        /// <summary>
        /// 最大ループ回数
        /// </summary>
        private int MaxLoopNum { get; set; }

        /// <summary>
        /// ループ停止時間（ミリ秒）
        /// </summary>
        private int SleepMillisec { get; set; }

        /// <summary>
        /// 監視設定SQL
        /// </summary>
        /// <remarks>
        /// テーブル名はスキーマ名から記述する必要あり。
        /// 検索カラムの*による省略不可。
        /// 
        /// 監視対象テーブルを複数のプロセスが1トランザクション内で複数回更新する場合、
        /// 監視専用のテーブルを別に用意したほうが良い。
        /// （Insertトリガーなどを使用して監視専用テーブルにデータを挿入しそちらを監視する。）
        /// 
        /// ■元テーブルと監視対象テーブルを別に分ける理由
        /// 監視対象テーブルが更新されると
        /// 更新対象データとともにDependecy用の自動生成されたデータも更新される。
        /// 複数トランザクションから2回以上ずつ更新される場合、
        /// 更新対象データとDependecy用のデータを別トランザクションが互い違いにつかみ合い
        /// デッドロックを起こすことがあるため。
        /// </remarks>
        private string Sql { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal DependencySample()
        {
            this.ConnectionString ="";
            this.MaxLoopNum = 60;
            this.SleepMillisec = 1000;
            this.Sql = "select column from dbo.MonitorTable";
        }

        /// <summary>
        /// メインループ処理
        /// </summary>
        public void Loop()
        {
            this.InitSqlDependency();
            this.SetSqlDependency();

            var loopCount = 0;
            while (true)
            {
                Thread.Sleep(1000);
                loopCount++;
                if (loopCount >= this.MaxLoopNum)
                {
                    this.DoSomething();
                }

                if (this.CheckExitLoop())
                {
                    break;
                }
            }

            this.TerminateSqlDependency();
        }

        /// <summary>
        /// SqlDependency初期化
        /// </summary>
        private void InitSqlDependency()
        {
            SqlDependency.Start(this.ConnectionString);
        }

        /// <summary>
        /// SqlDependency設定
        /// </summary>
        private void SetSqlDependency()
        {
            using (var conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                using (var command = new SqlCommand(this.Sql, conn))
                {
                    command.Notification = null;
                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(this.OnChangeDependency);

                    using (var reader = command.ExecuteReader())
                    {
                        // このタイミングで監視が開始されるので
                        // 下の処理実施中にOnChangeDependencyメソッドが呼び出される場合がある
                        // DoSomethingメソッドが重複実行不可の場合、何らかの制御が必要
                        this.DoSomething();
                    }

                }
            }
        }

        /// <summary>
        /// SqlDependency終了処理
        /// </summary>
        private void TerminateSqlDependency()
        {
            SqlDependency.Stop(this.ConnectionString);
        }

        /// <summary>
        /// 通知イベント発生時に呼び出されるメソッドです。
        /// 監視設定SQLの実行結果が変化した場合やエラー発生時に呼び出されます。
        /// </summary>
        /// <param name="sender">SqlDependencyオブジェクト</param>
        /// <param name="e">通知イベント情報</param>
        private void OnChangeDependency(object sender, SqlNotificationEventArgs e)
        {
            try
            {
                this.DoSomething();
                var dependency = sender as SqlDependency;
                dependency.OnChange -= this.OnChangeDependency;
                this.SetSqlDependency();
            }
            catch (Exception)
            {
                // ログ出力などのエラー処理を行う
            }
        }

        /// <summary>
        /// 更新を検知した場合に実行すべき処理をここに記載します。
        /// </summary>
        /// <remarks>
        /// 通知を受けた場合、
        /// 前の通知によって作成されたスレッドがデータを処理してしまい
        /// 更新対象データがない場合も想定した作りにする必要があります。
        /// </remarks>
        private void DoSomething()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ループを抜けるためのチェック処理を記述します。
        /// </summary>
        /// <returns></returns>
        private bool CheckExitLoop()
        {
            throw new NotImplementedException();
        }
    }
}
