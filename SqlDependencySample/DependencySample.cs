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
        /// コンストラクタ
        /// </summary>
        internal DependencySample()
        {
            this.ConnectionString ="";
            this.MaxLoopNum = 60;
            this.SleepMillisec = 1000;
        }

        /// <summary>
        /// メインループ処理
        /// </summary>
        public void Loop()
        {
            var loopCount = 0;
            while (true)
            {
                Thread.Sleep(1000);
                loopCount++;
                if (loopCount >= this.MaxLoopNum)
                {
                    this.DoSomething();
                }

                //// ループの停止処理をここに記載
            }
        }

        /// <summary>
        /// SqlDependency初期化
        /// </summary>
        private void Initialize()
        {
            SqlDependency.Start(this.ConnectionString);
        }

        /// <summary>
        /// 実処理をここに記載します。
        /// </summary>
        private void DoSomething()
        {
            
        }
    }
}
