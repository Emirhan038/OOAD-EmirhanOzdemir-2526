using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

namespace BusinessLayer
{
    // Abstract base shared by Patient and Dokter — holds every column that
    // appears in both DB tables: id, voornaam, achternaam, gsm, email,
    // paswoord (hashed), and profielfotodata (raw bytes from the image column).
    public abstract class Persoon
    {
        public int    Id              { get; protected set; }
        public string Voornaam        { get; set; }
        public string Achternaam      { get; set; }
        public string Gsm             { get; set; }   // nullable in DB (nchar 10)
        public string Email           { get; set; }
        public string Paswoord        { get; set; }   // stored as SHA-256 hex
        public byte[] Profielfotodata { get; set; }   // null when no photo

        // Returns lowercase hex SHA-256 of the plain-text password.
        // Used by both Patient and Dokter when saving or logging in.
        public static string HashPaswoord(string plaintext)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plaintext));

            // Build hex string without dashes — x2 gives two lowercase hex chars per byte.
            StringBuilder sb = new StringBuilder(64);
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        // Checks whether 'plaintext' matches the stored hash.
        public bool ControleerPaswoord(string plaintext)
        {
            return Paswoord == HashPaswoord(plaintext);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    //  PATIENT
    // ──────────────────────────────────────────────────────────────────────────
    public class Patient : Persoon
    {
        public Geslacht    Geslacht       { get; set; }
        public DateTime    Geboortedatum  { get; set; }
        public Notificatie Notificaties   { get; set; }

        // ── Read ──────────────────────────────────────────────────────────────

        // Loads all patients from the DB and returns them as a List<Patient>.
        public static List<Patient> GetAll()
        {
            List<Patient> lijst = new List<Patient>();

            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, " +
                    "       geboortedatum, profielfotodata, notificaties " +
                    "FROM   Patient",
                    conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lijst.Add(LeesRij(reader));
                }
                reader.Close();
            }
            finally
            {
                conn.Close();
            }

            return lijst;
        }

        // Returns the Patient with the given id, or null if not found.
        public static Patient GetById(int id)
        {
            Patient gevonden = null;

            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, " +
                    "       geboortedatum, profielfotodata, notificaties " +
                    "FROM   Patient " +
                    "WHERE  id = @id",
                    conn);
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    gevonden = LeesRij(reader);
                }
                reader.Close();
            }
            finally
            {
                conn.Close();
            }

            return gevonden;
        }

        // Returns the Patient with the given email, or null if not found.
        // Used during login to look up the account before checking the password.
        public static Patient GetByEmail(string email)
        {
            Patient gevonden = null;

            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, " +
                    "       geboortedatum, profielfotodata, notificaties " +
                    "FROM   Patient " +
                    "WHERE  email = @email",
                    conn);
                cmd.Parameters.AddWithValue("@email", email);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    gevonden = LeesRij(reader);
                }
                reader.Close();
            }
            finally
            {
                conn.Close();
            }

            return gevonden;
        }

        // ── Write ─────────────────────────────────────────────────────────────

        // Inserts this new Patient into the DB and sets Id from the generated key.
        // Paswoord must already be hashed before calling this method.
        public void Opslaan()
        {
            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Patient " +
                    "  (voornaam, achternaam, geslacht, gsm, email, paswoord, " +
                    "   geboortedatum, profielfotodata, notificaties) " +
                    "VALUES " +
                    "  (@voornaam, @achternaam, @geslacht, @gsm, @email, @paswoord, " +
                    "   @geboortedatum, @profielfotodata, @notificaties); " +
                    "SELECT SCOPE_IDENTITY();",
                    conn);

                VoegParametersToe(cmd);

                // SCOPE_IDENTITY() returns decimal; cast to int.
                Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            finally
            {
                conn.Close();
            }
        }

        // Updates every editable field for the already-persisted Patient (Id must be set).
        public void Bijwerken()
        {
            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Patient SET " +
                    "  voornaam        = @voornaam, " +
                    "  achternaam      = @achternaam, " +
                    "  geslacht        = @geslacht, " +
                    "  gsm             = @gsm, " +
                    "  email           = @email, " +
                    "  paswoord        = @paswoord, " +
                    "  geboortedatum   = @geboortedatum, " +
                    "  profielfotodata = @profielfotodata, " +
                    "  notificaties    = @notificaties " +
                    "WHERE id = @id",
                    conn);

                VoegParametersToe(cmd);
                cmd.Parameters.AddWithValue("@id", Id);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        // Deletes the patient and all linked Afspraak rows (FK constraint requires
        // child rows to be removed first — cascade deletes are not configured in this DB).
        public void Verwijderen()
        {
            SqlConnection conn = Database.OpenConnection();
            try
            {
                // Step 1: remove linked appointments.
                SqlCommand delAfspraak = new SqlCommand(
                    "DELETE FROM Afspraak WHERE patient_id = @id", conn);
                delAfspraak.Parameters.AddWithValue("@id", Id);
                delAfspraak.ExecuteNonQuery();

                // Step 2: remove the patient itself.
                SqlCommand delPatient = new SqlCommand(
                    "DELETE FROM Patient WHERE id = @id", conn);
                delPatient.Parameters.AddWithValue("@id", Id);
                delPatient.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        // Maps the current reader row to a Patient object.
        private static Patient LeesRij(SqlDataReader reader)
        {
            Patient p = new Patient();
            p.Id             = Convert.ToInt32(reader["id"]);
            p.Voornaam       = reader["voornaam"].ToString();
            p.Achternaam     = reader["achternaam"].ToString();
            p.Geslacht       = (Geslacht)Convert.ToInt32(reader["geslacht"]);
            p.Gsm            = reader["gsm"] == DBNull.Value ? null : reader["gsm"].ToString().Trim();
            p.Email          = reader["email"].ToString();
            p.Paswoord       = reader["paswoord"].ToString();
            p.Geboortedatum  = Convert.ToDateTime(reader["geboortedatum"]);
            p.Profielfotodata = reader["profielfotodata"] == DBNull.Value
                                ? null
                                : (byte[])reader["profielfotodata"];
            p.Notificaties   = (Notificatie)Convert.ToInt32(reader["notificaties"]);
            return p;
        }

        // Adds all INSERT/UPDATE parameters except @id.
        private void VoegParametersToe(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@voornaam",        Voornaam);
            cmd.Parameters.AddWithValue("@achternaam",      Achternaam);
            cmd.Parameters.AddWithValue("@geslacht",        (int)Geslacht);
            cmd.Parameters.AddWithValue("@gsm",             (object)Gsm ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email",           Email);
            cmd.Parameters.AddWithValue("@paswoord",        Paswoord);
            cmd.Parameters.AddWithValue("@geboortedatum",   Geboortedatum);
            cmd.Parameters.AddWithValue("@profielfotodata", (object)Profielfotodata ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@notificaties",    (int)Notificaties);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    //  DOKTER
    // ──────────────────────────────────────────────────────────────────────────
    public class Dokter : Persoon
    {
        public int  Rizivnummer        { get; set; }
        public bool IsGeconventioneerd { get; set; }  // tinyint 0/1 in DB

        // ── Read ──────────────────────────────────────────────────────────────

        // Loads all doctors from the DB.
        public static List<Dokter> GetAll()
        {
            List<Dokter> lijst = new List<Dokter>();

            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT id, voornaam, achternaam, gsm, email, paswoord, " +
                    "       profielfotodata, rizivnummer, isgeconventioneerd " +
                    "FROM   Dokter",
                    conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lijst.Add(LeesRij(reader));
                }
                reader.Close();
            }
            finally
            {
                conn.Close();
            }

            return lijst;
        }

        // Returns the Dokter with the given id, or null if not found.
        public static Dokter GetById(int id)
        {
            Dokter gevonden = null;

            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT id, voornaam, achternaam, gsm, email, paswoord, " +
                    "       profielfotodata, rizivnummer, isgeconventioneerd " +
                    "FROM   Dokter " +
                    "WHERE  id = @id",
                    conn);
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    gevonden = LeesRij(reader);
                }
                reader.Close();
            }
            finally
            {
                conn.Close();
            }

            return gevonden;
        }

        // Returns the Dokter with the given email, or null if not found.
        // Used during login to look up the account before checking the password.
        public static Dokter GetByEmail(string email)
        {
            Dokter gevonden = null;

            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT id, voornaam, achternaam, gsm, email, paswoord, " +
                    "       profielfotodata, rizivnummer, isgeconventioneerd " +
                    "FROM   Dokter " +
                    "WHERE  email = @email",
                    conn);
                cmd.Parameters.AddWithValue("@email", email);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    gevonden = LeesRij(reader);
                }
                reader.Close();
            }
            finally
            {
                conn.Close();
            }

            return gevonden;
        }

        // ── Write ─────────────────────────────────────────────────────────────

        // Inserts this new Dokter into the DB and sets Id from the generated key.
        public void Opslaan()
        {
            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Dokter " +
                    "  (voornaam, achternaam, gsm, email, paswoord, " +
                    "   profielfotodata, rizivnummer, isgeconventioneerd) " +
                    "VALUES " +
                    "  (@voornaam, @achternaam, @gsm, @email, @paswoord, " +
                    "   @profielfotodata, @rizivnummer, @isgeconventioneerd); " +
                    "SELECT SCOPE_IDENTITY();",
                    conn);

                VoegParametersToe(cmd);

                Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            finally
            {
                conn.Close();
            }
        }

        // Updates every editable field for the already-persisted Dokter.
        public void Bijwerken()
        {
            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Dokter SET " +
                    "  voornaam           = @voornaam, " +
                    "  achternaam         = @achternaam, " +
                    "  gsm                = @gsm, " +
                    "  email              = @email, " +
                    "  paswoord           = @paswoord, " +
                    "  profielfotodata    = @profielfotodata, " +
                    "  rizivnummer        = @rizivnummer, " +
                    "  isgeconventioneerd = @isgeconventioneerd " +
                    "WHERE id = @id",
                    conn);

                VoegParametersToe(cmd);
                cmd.Parameters.AddWithValue("@id", Id);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static Dokter LeesRij(SqlDataReader reader)
        {
            Dokter d = new Dokter();
            d.Id                  = Convert.ToInt32(reader["id"]);
            d.Voornaam            = reader["voornaam"].ToString();
            d.Achternaam          = reader["achternaam"].ToString();
            d.Gsm                 = reader["gsm"] == DBNull.Value ? null : reader["gsm"].ToString().Trim();
            d.Email               = reader["email"].ToString();
            d.Paswoord            = reader["paswoord"].ToString();
            d.Profielfotodata     = reader["profielfotodata"] == DBNull.Value
                                    ? null
                                    : (byte[])reader["profielfotodata"];
            d.Rizivnummer         = Convert.ToInt32(reader["rizivnummer"]);
            d.IsGeconventioneerd  = Convert.ToInt32(reader["isgeconventioneerd"]) == 1;
            return d;
        }

        private void VoegParametersToe(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@voornaam",           Voornaam);
            cmd.Parameters.AddWithValue("@achternaam",         Achternaam);
            cmd.Parameters.AddWithValue("@gsm",                (object)Gsm ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email",              Email);
            cmd.Parameters.AddWithValue("@paswoord",           Paswoord);
            cmd.Parameters.AddWithValue("@profielfotodata",    (object)Profielfotodata ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@rizivnummer",        Rizivnummer);
            cmd.Parameters.AddWithValue("@isgeconventioneerd", IsGeconventioneerd ? 1 : 0);
        }
    }
}
