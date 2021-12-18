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
                // You need to create two files (ethAddress.txt and l2Pk.txt) containing your ethereum public address and your loopring private key respectively
                // This is a temporary inconvenience that I'm working on sorting out in the future, so all that you put in is the loopring api url and nothing more
                LoopringAPI.Client client = new Client("https://uat2.loopring.io/", File.ReadAllText("ethAddress.txt"), File.ReadAllText("l2Pk.txt"), true);
                var result = client.Transfer("0x2e76ebd1c7c0c8e7c2b875b6d505a260c525d25e", "LRC", 1, "LRC", "Potatoes are nice").Result;
                MessageBox.Show("Thanks for the monye honey!");
            
        }
    }
}