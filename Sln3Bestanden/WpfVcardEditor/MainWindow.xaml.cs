using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfVcardEditor
{
    public partial class MainWindow : Window
    {
        private string? _currentFile = null;
        private bool _hasChanges = false;
        private bool _isLoading = false;
        private string? _photoBase64 = null;

        public MainWindow()
        {
            InitializeComponent();
            WireChangeEvents();
        }

        private void WireChangeEvents()
        {
            txtFirstname.TextChanged += (s, e) => Card_Changed();
            txtLastname.TextChanged += (s, e) => Card_Changed();
            datBirthday.SelectedDateChanged += (s, e) => Card_Changed();
            radFemale.Checked += (s, e) => Card_Changed();
            radMale.Checked += (s, e) => Card_Changed();
            radUnknown.Checked += (s, e) => Card_Changed();
            txtPrivateEmail.TextChanged += (s, e) => Card_Changed();
            txtPrivatePhone.TextChanged += (s, e) => Card_Changed();
            txtCompany.TextChanged += (s, e) => Card_Changed();
            txtJobTitle.TextChanged += (s, e) => Card_Changed();
            txtWorkEmail.TextChanged += (s, e) => Card_Changed();
            txtWorkPhone.TextChanged += (s, e) => Card_Changed();
            txtLinkedIn.TextChanged += (s, e) => Card_Changed();
            txtFacebook.TextChanged += (s, e) => Card_Changed();
            txtInstagram.TextChanged += (s, e) => Card_Changed();
            txtYoutube.TextChanged += (s, e) => Card_Changed();
        }

        private void Card_Changed()
        {
            if (_isLoading) return;
            _hasChanges = true;
            UpdateStatus();
        }

        private void ClearForm()
        {
            txtFirstname.Text = "";
            txtLastname.Text = "";
            datBirthday.SelectedDate = null;
            radUnknown.IsChecked = true;
            txtPrivateEmail.Text = "";
            txtPrivatePhone.Text = "";
            lblPhotoPath.Text = "(geen geselecteerd)";
            imgPhoto.Source = null;
            _photoBase64 = null;
            txtCompany.Text = "";
            txtJobTitle.Text = "";
            txtWorkEmail.Text = "";
            txtWorkPhone.Text = "";
            txtLinkedIn.Text = "";
            txtFacebook.Text = "";
            txtInstagram.Text = "";
            txtYoutube.Text = "";
        }

        private void UpdateStatus()
        {
            if (_currentFile == null)
            {
                txtStatus.Text = "huidige kaart: (geen geopend)";
                txtPercentage.Text = "percentage ingevuld: n.a.";
            }
            else
            {
                txtStatus.Text = $"huidige kaart: {_currentFile}";
                txtPercentage.Text = $"percentage ingevuld: {CalculatePercentage()}%";
            }
        }

        private int CalculatePercentage()
        {
            int total = 0;
            int filled = 0;

            void CheckText(string value)
            {
                total++;
                if (!string.IsNullOrWhiteSpace(value)) filled++;
            }

            CheckText(txtFirstname.Text);
            CheckText(txtLastname.Text);
            total++; if (datBirthday.SelectedDate.HasValue) filled++;
            total++; filled++;
            CheckText(txtPrivateEmail.Text);
            CheckText(txtPrivatePhone.Text);
            total++; if (imgPhoto.Source != null) filled++;
            CheckText(txtCompany.Text);
            CheckText(txtJobTitle.Text);
            CheckText(txtWorkEmail.Text);
            CheckText(txtWorkPhone.Text);
            CheckText(txtLinkedIn.Text);
            CheckText(txtFacebook.Text);
            CheckText(txtInstagram.Text);
            CheckText(txtYoutube.Text);

            return total == 0 ? 0 : (int)Math.Round((double)filled / total * 100);
        }

        private void LoadCard(string path)
        {
            string[] rawLines = File.ReadAllLines(path, Encoding.UTF8);
            List<string> lines = new List<string>();

            foreach (string rawLine in rawLines)
            {
                if ((rawLine.StartsWith(" ") || rawLine.StartsWith("\t")) && lines.Count > 0)
                    lines[lines.Count - 1] += rawLine.TrimStart();
                else
                    lines.Add(rawLine);
            }

            _isLoading = true;
            ClearForm();

            foreach (string line in lines)
            {
                int colonIndex = line.IndexOf(':');
                if (colonIndex < 0) continue;

                string key = line.Substring(0, colonIndex).ToUpper();
                string value = line.Substring(colonIndex + 1);

                if (key == "N" || key.StartsWith("N;"))
                {
                    string[] parts = value.Split(';');
                    txtLastname.Text = parts.Length > 0 ? parts[0] : "";
                    txtFirstname.Text = parts.Length > 1 ? parts[1] : "";
                }
                else if (key == "GENDER")
                {
                    if (value == "F") radFemale.IsChecked = true;
                    else if (value == "M") radMale.IsChecked = true;
                    else radUnknown.IsChecked = true;
                }
                else if (key == "BDAY")
                {
                    string[] formats = { "yyyyMMdd", "yyyy-MM-dd" };
                    if (DateTime.TryParseExact(value, formats, null, DateTimeStyles.None, out DateTime bday))
                        datBirthday.SelectedDate = bday;
                }
                else if (key.Contains("EMAIL") && key.Contains("HOME"))
                {
                    txtPrivateEmail.Text = value;
                }
                else if (key.Contains("EMAIL") && key.Contains("WORK"))
                {
                    txtWorkEmail.Text = value;
                }
                else if (key.Contains("TEL") && key.Contains("HOME"))
                {
                    txtPrivatePhone.Text = value;
                }
                else if (key.Contains("TEL") && key.Contains("WORK"))
                {
                    txtWorkPhone.Text = value;
                }
                else if (key == "TITLE" || key.StartsWith("TITLE;"))
                {
                    txtJobTitle.Text = value;
                }
                else if (key == "ORG" || key.StartsWith("ORG;"))
                {
                    txtCompany.Text = value;
                }
                else if (key.Contains("PHOTO"))
                {
                    _photoBase64 = value;
                    try
                    {
                        byte[] imageBytes = Convert.FromBase64String(value);
                        BitmapImage bitmap = new BitmapImage();
                        using MemoryStream ms = new MemoryStream(imageBytes);
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        imgPhoto.Source = bitmap;
                        lblPhotoPath.Text = "(uit bestand)";
                    }
                    catch (FormatException)
                    {
                    }
                }
                else if (key.Contains("X-SOCIALPROFILE") && key.Contains("FACEBOOK"))
                {
                    txtFacebook.Text = value;
                }
                else if (key.Contains("X-SOCIALPROFILE") && key.Contains("LINKEDIN"))
                {
                    txtLinkedIn.Text = value;
                }
                else if (key.Contains("X-SOCIALPROFILE") && key.Contains("INSTAGRAM"))
                {
                    txtInstagram.Text = value;
                }
                else if (key.Contains("X-SOCIALPROFILE") && key.Contains("YOUTUBE"))
                {
                    txtYoutube.Text = value;
                }
            }

            _isLoading = false;
            _currentFile = path;
            _hasChanges = false;
            mnuSave.IsEnabled = true;
            UpdateStatus();
        }

        private void SaveCard(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCARD");
            sb.AppendLine("VERSION:3.0");

            string firstName = txtFirstname.Text.Trim();
            string lastName = txtLastname.Text.Trim();
            string fullName = (firstName + " " + lastName).Trim();
            if (!string.IsNullOrEmpty(fullName))
            {
                sb.AppendLine($"FN;CHARSET=UTF-8:{fullName}");
                sb.AppendLine($"N;CHARSET=UTF-8:{lastName};{firstName};;;");
            }

            if (radFemale.IsChecked == true) sb.AppendLine("GENDER:F");
            else if (radMale.IsChecked == true) sb.AppendLine("GENDER:M");

            if (datBirthday.SelectedDate.HasValue)
                sb.AppendLine($"BDAY:{datBirthday.SelectedDate.Value:yyyyMMdd}");

            if (!string.IsNullOrWhiteSpace(txtPrivateEmail.Text))
                sb.AppendLine($"EMAIL;CHARSET=UTF-8;type=HOME,INTERNET:{txtPrivateEmail.Text.Trim()}");
            if (!string.IsNullOrWhiteSpace(txtWorkEmail.Text))
                sb.AppendLine($"EMAIL;CHARSET=UTF-8;type=WORK,INTERNET:{txtWorkEmail.Text.Trim()}");

            if (_photoBase64 != null)
                sb.AppendLine($"PHOTO;ENCODING=b;TYPE=JPEG:{_photoBase64}");

            if (!string.IsNullOrWhiteSpace(txtPrivatePhone.Text))
                sb.AppendLine($"TEL;TYPE=HOME,VOICE:{txtPrivatePhone.Text.Trim()}");
            if (!string.IsNullOrWhiteSpace(txtWorkPhone.Text))
                sb.AppendLine($"TEL;TYPE=WORK,VOICE:{txtWorkPhone.Text.Trim()}");

            if (!string.IsNullOrWhiteSpace(txtJobTitle.Text))
                sb.AppendLine($"TITLE;CHARSET=UTF-8:{txtJobTitle.Text.Trim()}");
            if (!string.IsNullOrWhiteSpace(txtCompany.Text))
                sb.AppendLine($"ORG;CHARSET=UTF-8:{txtCompany.Text.Trim()}");

            if (!string.IsNullOrWhiteSpace(txtFacebook.Text))
                sb.AppendLine($"X-SOCIALPROFILE;TYPE=facebook:{txtFacebook.Text.Trim()}");
            if (!string.IsNullOrWhiteSpace(txtLinkedIn.Text))
                sb.AppendLine($"X-SOCIALPROFILE;TYPE=linkedin:{txtLinkedIn.Text.Trim()}");
            if (!string.IsNullOrWhiteSpace(txtInstagram.Text))
                sb.AppendLine($"X-SOCIALPROFILE;TYPE=instagram:{txtInstagram.Text.Trim()}");
            if (!string.IsNullOrWhiteSpace(txtYoutube.Text))
                sb.AppendLine($"X-SOCIALPROFILE;TYPE=youtube:{txtYoutube.Text.Trim()}");

            sb.AppendLine($"REV:{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}");
            sb.AppendLine("END:VCARD");

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        private void mnuNew_Click(object sender, RoutedEventArgs e)
        {
            if (_hasChanges)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Er zijn niet-opgeslagen wijzigingen. Wil je doorgaan zonder op te slaan?",
                    "Bevestiging",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.No) return;
            }

            _isLoading = true;
            ClearForm();
            _isLoading = false;
            _currentFile = null;
            _hasChanges = false;
            mnuSave.IsEnabled = false;
            UpdateStatus();
        }

        private void mnuOpen_Click(object sender, RoutedEventArgs e)
        {
            if (_hasChanges)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Er zijn niet-opgeslagen wijzigingen. Wil je doorgaan zonder op te slaan?",
                    "Bevestiging",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.No) return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "VCard bestanden (*.vcf)|*.vcf";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    LoadCard(dialog.FileName);
                }
                catch (IOException)
                {
                    MessageBox.Show(
                        $"Kan bestand {dialog.FileName} niet lezen",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Er is een onverwachte fout opgetreden:\n{ex.Message}",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void mnuSave_Click(object sender, RoutedEventArgs e)
        {
            if (_currentFile == null) return;

            try
            {
                SaveCard(_currentFile);
                _hasChanges = false;
                MessageBox.Show(
                    "Bestand opgeslagen.",
                    "Opgeslagen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show(
                    $"Kan bestand niet opslaan.\n{ex.Message}",
                    "FOUT",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Er is een onverwachte fout opgetreden:\n{ex.Message}",
                    "FOUT",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void mnuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "VCard bestanden (*.vcf)|*.vcf";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    SaveCard(dialog.FileName);
                    _currentFile = dialog.FileName;
                    _hasChanges = false;
                    mnuSave.IsEnabled = true;
                    UpdateStatus();
                }
                catch (IOException ex)
                {
                    MessageBox.Show(
                        $"Kan bestand niet opslaan.\n{ex.Message}",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Er is een onverwachte fout opgetreden:\n{ex.Message}",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.Owner = this;
            about.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Ben je zeker dat je de applicatie wil afsluiten?",
                "Toepassing sluiten",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
                e.Cancel = true;
        }

        private void btnSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Afbeeldingen (*.jpg;*.jpeg)|*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    lblPhotoPath.Text = dialog.FileName;
                    BitmapImage bitmap = new BitmapImage(new Uri(dialog.FileName));
                    imgPhoto.Source = bitmap;
                    byte[] imageBytes = File.ReadAllBytes(dialog.FileName);
                    _photoBase64 = Convert.ToBase64String(imageBytes);
                    Card_Changed();
                }
                catch (IOException ex)
                {
                    MessageBox.Show(
                        $"Kan afbeelding niet laden.\n{ex.Message}",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Er is een onverwachte fout opgetreden:\n{ex.Message}",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
    }
}
