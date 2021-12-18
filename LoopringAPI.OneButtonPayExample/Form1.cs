namespace LoopringAPI.OneButtonPayExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoopringAPI.Client client = new Client("https://uat2.loopring.io/", null, File.ReadAllText("l2Pk.txt"));
            client.Transfer("0x452386e0516cc1600e9f43c719d0c80c6abc51f9", "LRC", 1, "LRC", "Fine you beggar, here!");
        }
    }
}