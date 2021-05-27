using AnkaKafe.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnkaKafe.UI
{
    public partial class UrunlerForm : Form
    {
        private readonly KafeVeri _db;
        private readonly BindingList<Urun> _blUrunler;
        public UrunlerForm(KafeVeri db)
        {
            _db = db;
            _blUrunler = new BindingList<Urun>(_db.Urunler);
            InitializeComponent();
            UrunleriListele();
        }

        private void UrunleriListele()
        {
            dgvUrunler.DataSource = _blUrunler;
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string urunAd = txtUrunAd.Text.Trim();

            
            if (urunAd == "")
            {
                MessageBox.Show("Lutfen bir urun adi giriniz.");
                return;
            }
            if (_duzenlenen == null) // duzenlenen yoksa ekle 
            {
            _blUrunler.Add(new Urun()
            {
                UrunAd = urunAd,
                BirimFiyat = nudBirimFiyat.Value
            });

            }
            else //varsa guncelle
            {
                _duzenlenen.UrunAd = urunAd;
                _duzenlenen.BirimFiyat = nudBirimFiyat.Value;
                _blUrunler.ResetBindings();
            }
            EkleFormunuSifirla();
        }

        private void EkleFormunuSifirla()
        {
            txtUrunAd.Clear();
            nudBirimFiyat.Value = 0;
            btnEkle.Text = "EKLE";
            btnIptal.Hide();
            _duzenlenen = null;
        }

        private void dgvUrunler_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && dgvUrunler.SelectedRows.Count > 0)
            {
                DialogResult dr = MessageBox.Show("Secili urun silinecektir. Onayliyor musunuz?","Urun Silme Onayi", MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button2);
                if (true)
                {
                Urun urun = (Urun)dgvUrunler.SelectedRows[0].DataBoundItem;
                _blUrunler.Remove(urun);

                }
            }
        }

        Urun _duzenlenen;
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (dgvUrunler.SelectedRows.Count == 0)
            {
                MessageBox.Show("Urun guncellemek icin once bir urun secmelisiniz.");
                return;
            }

            _duzenlenen = (Urun)dgvUrunler.SelectedRows[0].DataBoundItem;
            txtUrunAd.Text = _duzenlenen.UrunAd;
            nudBirimFiyat.Value = _duzenlenen.BirimFiyat;
            btnEkle.Text = "KAYDET";
            btnIptal.Show();
        }
    }
}
