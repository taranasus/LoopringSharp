using LoopringSharp;

namespace LoopringSharp.OneButtonPayExample
{
    public partial class TransferForm : Form
    {
        public TransferForm()
        {
            InitializeComponent();
        }

        MetaMask.MetamaskClient client;

        // When user selects a network to connect to
        private void cbEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Take their selection, remove the extra text leaving only the url, and send that to the LoopringSharp Client
            // Since only the URL was provided and no other info, the client will attempt to make contact with the users's MetaMask in order
            // To get the necesairy info
            //                  The url of the exchange you will be interacting with   Which wallet connecrtion method are we using?
            client = new MetaMask.MetamaskClient(cbEnvironment.SelectedItem.ToString().Split(" | ")[1]);

            // Clear the textboxes in case this is a reload
            cbPaymentFeeToken.Items.Clear();
            cbPaymentToken.Items.Clear();

            // Change the connection lable to show which network we are connected to
            connectionStatus.Text = "Connected: " + cbEnvironment.SelectedItem.ToString();

            // Use the LoopringSharp to get all the available tokens that can be transfered
            var tokenOptions = client.GetTokens();
            // Get the tokens from above and add them as payment options
            cbPaymentToken.Items.AddRange(tokenOptions.Select(s => s.symbol).ToArray());
            // Set the LRC token as the default option
            cbPaymentToken.SelectedIndex = 1;

            // Enable all the UI items in case they are disabled.
            cbPaymentToken.Enabled = true;
            tbPayeeAddress.Enabled = true;
            cbPaymentFeeToken.Enabled = true;
            nudPaymentAmmount.Enabled = true;
            btnMakePayment.Enabled = true;

            // Bring the form to the front;
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Focus();
        }   

        private void cbPaymentToken_SelectedIndexChanged(object sender, EventArgs e)
        {
            // A different payment token was selected!
            if (cbPaymentFeeToken.Items.Count == 0)
            {
                // Use the LoopringSharp to get all the tokens that we're allowed to pay transaction fees in
                //                                 Type of action you'll do      // Token you'll be doing it in          // The ammount for that token
                var feeTokens = client.OffchainFee(OffChainRequestType.Transfer, cbPaymentToken.SelectedItem.ToString(), nudPaymentAmmount.Value.ToString());
                // Add those tokens to the payment token combobox
                cbPaymentFeeToken.Items.AddRange(feeTokens.fees.Select(s => s.token).ToArray());
                // Select the first token as the default
                cbPaymentFeeToken.SelectedIndex = 0;
            }
            // Make another call to get the feel values for the transfer token
            SetTransferTokenFee();
        }

        private void cbPaymentFeeToken_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTransferTokenFee();
        }

        void SetTransferTokenFee()
        {
            // Get which item we've selected from the dropdown list
            var selectedItem = cbPaymentFeeToken.Items[cbPaymentFeeToken.SelectedIndex].ToString();
            // Get the fee tokens agian from the API
            //                                 Type of action you'll do      // Token you'll be doing it in          // The ammount for that token
            var feeTokens = client.OffchainFee(OffChainRequestType.Transfer, cbPaymentToken.SelectedItem.ToString(), nudPaymentAmmount.Value.ToString());
            // For the token that the user has selected, display it's resepctive fee on the screen
            lbTransactionFee.Text = "Transfer Fee: "
                + feeTokens.fees.Where(w => w.token == selectedItem).FirstOrDefault().normalziedFee + " "
                + feeTokens.fees.Where(w => w.token == selectedItem).FirstOrDefault().token;
        }

        private void btnMakePayment_Click(object sender, EventArgs e)
        {
            // Request the transfer to be made, using the user's inputed details
            //                           Who you paying       Which currency you are paying in        How much you are paying  Which currency you are paying the fee in   Message attached to payment
            var result = client.Transfer(tbPayeeAddress.Text, cbPaymentToken.SelectedItem.ToString(), nudPaymentAmmount.Value, cbPaymentFeeToken.SelectedItem.ToString(), tbMemo.Text);

            // Endpoint not yet implemented
            //var status = client.Transfers().Result;

            //while(status.status != OrderStatus.processed)
            //{
            //    System.Threading.Thread.Sleep(100);
            //}

            // Bring the form to the front;            
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal; 
            this.Focus();
            MessageBox.Show("Thanks for the monye honey! I will use it to buy skittles!");
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}