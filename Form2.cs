using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Practice_Rx_Cs_01
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var provider = GetProvider1();

            provider.SubscribeOn(ThreadPoolScheduler.Instance) //Subscribeするスケジューラを指定
                .TakeUntil(ButtonClickAsObservable(this.btnStop)) //ボタンが押されるとonCompleteとなる  
                .ObserveOn(System.Threading.SynchronizationContext.Current)//onNextするスケジューラを指定
                .Subscribe(x =>
                {
                    this.button1.Enabled = false;
                    this.textBox1.Text = x;
                }
                , () =>
                { //onComplete  
                    this.button1.Enabled = true;
                    this.textBox1.Text = "処理終了";
                }
                );

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var provider = GetProvider2();

            provider.SubscribeOn(ThreadPoolScheduler.Instance) //Subscribeするスケジューラを指定
                .TakeUntil(ButtonClickAsObservable(this.btnStop)) //ボタンが押されるとonCompleteとなる  
                .Sample(TimeSpan.FromMilliseconds(300)) //直近300msで最新の値を使用（→300ms毎に更新)
                .ObserveOn(System.Threading.SynchronizationContext.Current)//onNextするスケジューラを指定
                .Subscribe(x =>
                {
                    this.button1.Enabled = false;
                    this.textBox1.Text = x;
                }
                , () =>
                { //onComplete  
                    this.button1.Enabled = true;
                    this.textBox1.Text = "処理終了";
                }
                );
        }

        private IObservable<string> GetProvider1()
        {

            IObservable<string> provider = Observable.Create<string>(o => {

                var listCount = 10000; //処理件数の合計  
                for (var i = 1; i <= listCount; i++)
                {
                    o.OnNext(string.Format("{0}/{1}", i, listCount)); //   n/N　形式の文字列を通知  
                    System.Threading.Thread.Sleep(1000); //時間のかかる処理と仮定  
                }
                o.OnCompleted(); //処理終了の通知  

                return System.Reactive.Disposables.Disposable.Empty; //何も返さないDisposable  
            });

            return provider;

        }

        private IObservable<string> GetProvider2()
        {
            var random = new System.Random();
            IObservable<string> provider = Observable.Create<string>(o => {

                var listCount = 10000; //処理件数の合計  
                for (var i = 1; i <= listCount; i++)
                {
                    o.OnNext(string.Format("{0}/{1}", i, listCount)); //   n/N　形式の文字列を通知  
                    System.Threading.Thread.Sleep(random.Next(1,100) ); //Sleep時間をランダムに
                }
                o.OnCompleted(); //処理終了の通知  

                return System.Reactive.Disposables.Disposable.Empty; //何も返さないDisposable  
            });

            return provider;

        }


        #region イベントからの変換はメソッドに退避  
        IObservable<EventArgs> ButtonClickAsObservable(Button btn)
        {
            return Observable.FromEvent<EventHandler, EventArgs>
            (
                handler => (sender, e) => handler(e),
                handler => btn.Click += handler,
                handler => btn.Click -= handler
            );
        }

        #endregion

        
    }
}
