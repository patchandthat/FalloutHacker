using System;
using System.Windows.Forms;
using FalloutHacker;

namespace HackerApp
{
    public partial class Form1 : Form
    {
        private ITerminalSolver _solver = new TerminalSolver();

        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_solver == null) return;

            lstPasswords.Items.Clear();

            foreach (string password in _solver.PossiblePasswords)
            {
                lstPasswords.Items.Add(password);
            }

            txtBestGuess.Text = _solver.BestGuess;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            _solver = new TerminalSolver();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (_solver == null) return;

            var pass = txtAdd.Text;

            try
            {
                _solver.AddPassword(pass);
            }
            catch (InvalidPasswordException ex)
            {
                MessageBox.Show(ex.Message);
            }

            txtAdd.Text = "";
        }

        private void btnEliminate_Click(object sender, EventArgs e)
        {
            if (_solver == null) return;

            int likeness;
            if (!int.TryParse(txtLikeness.Text, out likeness))
            {
                MessageBox.Show("Likeness must be an integer");
                return;
            }

            if (likeness < 0 || likeness > txtBestGuess.Text.Length)
            {
                MessageBox.Show("Likeness must be between 0 and the length of the password");
                return;
            }

            try
            {
                var pass = _solver.BestGuess;
                _solver.EliminatePassword(pass, likeness);
            }
            catch (InvalidPasswordException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtAdd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
            {
                btnAdd_Click(this, EventArgs.Empty);
                e.Handled = true;
            }
        }

        private void txtLikeness_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnEliminate_Click(this, EventArgs.Empty);
                e.Handled = true;
            }
        }
    }
}
