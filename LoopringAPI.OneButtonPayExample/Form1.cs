namespace LoopringAPI.OneButtonPayExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public bool paymentReceived = false;

        private void button1_Click(object sender, EventArgs e)
        {
                LoopringAPI.Client client = new Client("https://uat2.loopring.io/", "0x452386e0516cc1600e9f43c719d0c80c6abc51f9", File.ReadAllText("l2Pk.txt"), true);
                var result = client.Transfer("0x2e76ebd1c7c0c8e7c2b875b6d505a260c525d25e", "LRC", 1, "LRC", "Potatoes are nice").Result;
                MessageBox.Show("Thanks for the monye honey!");
            
        }
    }
}