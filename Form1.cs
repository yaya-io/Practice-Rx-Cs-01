using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Practice_Rx_Cs_01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Drawing2();
        }
        private void Drawing1()
        {
            //dragのObserveble
            var drag = this.MouseDownAsObservable()
                        .SelectMany(_ => this.MouseMoveAsObservable())
                        .TakeUntil(this.MouseUpAsObservable());

            drag.Zip //--- 前後の値をZipでまとめる
            (
                drag.Skip(1),
                (x, y) => new { Prev = x.Location, Next = y.Location }
            )
            .Repeat() //--- 何度も繰り返すためRepeat
            .Delay(TimeSpan.FromMilliseconds(1000)) //---描画を1秒遅らせる
            .Subscribe(location =>
            {
                //--- 前の点と次の点を直線で結ぶ  
                using (var graphic = this.CreateGraphics())
                using (var pen = new Pen(Color.Red, 3))
                    graphic.DrawLine(pen, location.Prev, location.Next);
            });
        }

        private void Drawing2()
        {
            //dragのObserveble
            var drag = this.MouseDownAsObservable()
                        .SelectMany(_ => this.MouseMoveAsObservable())
                        .TakeUntil(this.MouseUpAsObservable());
            drag.Zip  //--- 前後の値をZipでまとめる 
            (
                drag.Skip(1),
                (x, y) => new { Prev = x.Location, Next = y.Location }
            )
            .Repeat() //--- 何度も繰り返すためRepeat              
            .Publish(xs => //--- Publishでストリームを分岐する  
            {
                //1秒送れて描画
                xs.Delay(TimeSpan.FromMilliseconds(1000))
                .Subscribe(location =>
                {                
                    using (var graphic = this.CreateGraphics())
                    using (var pen = new Pen(Color.Red, 3))
                        graphic.DrawLine(pen, location.Prev, location.Next);
                });

                //2秒送れて描画
                xs.Delay(TimeSpan.FromMilliseconds(2000))
               .Subscribe(location =>
               {                    
                    using (var graphic = this.CreateGraphics())
                    using (var pen = new Pen(Color.Blue, 3))
                       graphic.DrawLine(pen, location.Prev, location.Next);
               });               

                return xs;
            }).Subscribe();

        }

        #region イベントからの変換はメソッドに退避  
        IObservable<MouseEventArgs> MouseDownAsObservable()
        {
            return Observable.FromEvent<MouseEventHandler, MouseEventArgs>
            (
                handler => (sender, e) => handler(e),
                handler => this.MouseDown += handler,
                handler => this.MouseDown -= handler
            );
        }

        IObservable<MouseEventArgs> MouseMoveAsObservable()
        {
            return Observable.FromEvent<MouseEventHandler, MouseEventArgs>
            (
                handler => (sender, e) => handler(e),
                handler => this.MouseMove += handler,
                handler => this.MouseMove -= handler
            );
        }

        IObservable<MouseEventArgs> MouseUpAsObservable()
        {
            return Observable.FromEvent<MouseEventHandler, MouseEventArgs>
            (
                handler => (sender, e) => handler(e),
                handler => this.MouseUp += handler,
                handler => this.MouseUp -= handler
            );
        }
        #endregion
    }
}
