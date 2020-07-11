using System;
using System.Threading;
using System.Windows.Forms;

namespace KeepAwake
{
    public partial class MainForm : Form
    {
        private int WM_SYSCOMMAND = 0x112;
        private IntPtr SC_MINIMIZE = (IntPtr)0xF020;
        private MouseMover mouseMover = new MouseMover();
        private Thread workerThread;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            //最小化されたときにフォームを非表示にする
            if ((m.Msg == WM_SYSCOMMAND) && (m.WParam == SC_MINIMIZE)) {
                this.Hide();
            }
            //上記以外はデフォルトの処理をおこなう
            else {
                base.WndProc(ref m);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //フォームを閉じる時、タスクトレイに表示されている場合は閉じずに非表示
            if (notifyIcon1.Visible) {
                e.Cancel = true;
                Hide();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mouseMover.RequestStop();
            workerThread.Join();
            notifyIcon1.Visible = false; // アイコンをトレイから取り除く
            Application.Exit(); // アプリケーションの終了
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //タスクトレイのアイコンダブルクリックでフォームを表示、アクティブ化
            WindowState = FormWindowState.Normal;
            Show();
            Activate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            workerThread = new Thread(mouseMover.Move);
            workerThread.Start();
            while (!workerThread.IsAlive) ;
        }
    }
}