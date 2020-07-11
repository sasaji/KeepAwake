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
            //�ŏ������ꂽ�Ƃ��Ƀt�H�[�����\���ɂ���
            if ((m.Msg == WM_SYSCOMMAND) && (m.WParam == SC_MINIMIZE)) {
                this.Hide();
            }
            //��L�ȊO�̓f�t�H���g�̏����������Ȃ�
            else {
                base.WndProc(ref m);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //�t�H�[������鎞�A�^�X�N�g���C�ɕ\������Ă���ꍇ�͕����ɔ�\��
            if (notifyIcon1.Visible) {
                e.Cancel = true;
                Hide();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mouseMover.RequestStop();
            workerThread.Join();
            notifyIcon1.Visible = false; // �A�C�R�����g���C�����菜��
            Application.Exit(); // �A�v���P�[�V�����̏I��
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //�^�X�N�g���C�̃A�C�R���_�u���N���b�N�Ńt�H�[����\���A�A�N�e�B�u��
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