using System;
using Microsoft.Data.SqlClient;

namespace BusinessLayer
{
    // Represents a single appointment row in the Afspraak table.
    // Cancelling an appointment = deleting the row (hard delete, per college spec).
    public class Afspraak
    {
        public int      Id         { get; private set; }
        public DateTime Moment     { get; set; }
        public string   Klacht     { get; set; }
        public int      PatientId  { get; set; }
        public int      DokterId   { get; set; }

        // ── Read ──────────────────────────────────────────────────────────────

        // Returns all appointments for the given doctor, ordered by datetime.
        public static List<Afspraak> GetByDokter(int dokterId)
        {
            List<Afspraak> lijst = new List<Afspraak>();

            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT id, moment, klacht, patient_id, dokter_id " +
                    "FROM   Afspraak " +
                    "WHERE  dokter_id = @dokterId " +
                    "ORDER BY moment",
                    conn);
                cmd.Parameters.AddWithValue("@dokterId", dokterId);

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

        // Returns all appointments for the given patient, ordered by datetime.
        public static List<Afspraak> GetByPatient(int patientId)
        {
            List<Afspraak> lijst = new List<Afspraak>();

            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT id, moment, klacht, patient_id, dokter_id " +
                    "FROM   Afspraak " +
                    "WHERE  patient_id = @patientId " +
                    "ORDER BY moment",
                    conn);
                cmd.Parameters.AddWithValue("@patientId", patientId);

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

        // Returns a single Afspraak by primary key, or null if not found.
        public static Afspraak GetById(int id)
        {
            Afspraak gevonden = null;

            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT id, moment, klacht, patient_id, dokter_id " +
                    "FROM   Afspraak " +
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

        // ── Write ─────────────────────────────────────────────────────────────

        // Inserts this Afspraak and sets Id from the generated key.
        public void Opslaan()
        {
            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Afspraak (moment, klacht, patient_id, dokter_id) " +
                    "VALUES (@moment, @klacht, @patientId, @dokterId); " +
                    "SELECT SCOPE_IDENTITY();",
                    conn);

                cmd.Parameters.AddWithValue("@moment",    Moment);
                cmd.Parameters.AddWithValue("@klacht",    Klacht);
                cmd.Parameters.AddWithValue("@patientId", PatientId);
                cmd.Parameters.AddWithValue("@dokterId",  DokterId);

                Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            finally
            {
                conn.Close();
            }
        }

        // Updates moment and klacht for an existing Afspraak.
        public void Bijwerken()
        {
            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Afspraak SET " +
                    "  moment    = @moment, " +
                    "  klacht    = @klacht, " +
                    "  patient_id = @patientId, " +
                    "  dokter_id  = @dokterId " +
                    "WHERE id = @id",
                    conn);

                cmd.Parameters.AddWithValue("@moment",    Moment);
                cmd.Parameters.AddWithValue("@klacht",    Klacht);
                cmd.Parameters.AddWithValue("@patientId", PatientId);
                cmd.Parameters.AddWithValue("@dokterId",  DokterId);
                cmd.Parameters.AddWithValue("@id",        Id);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        // Cancelling = hard delete of the row (no status column in this DB).
        public void Annuleren()
        {
            SqlConnection conn = Database.OpenConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Afspraak WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        // ── Helper ────────────────────────────────────────────────────────────

        private static Afspraak LeesRij(SqlDataReader reader)
        {
            Afspraak a = new Afspraak();
            a.Id        = Convert.ToInt32(reader["id"]);
            a.Moment    = Convert.ToDateTime(reader["moment"]);
            a.Klacht    = reader["klacht"].ToString();
            a.PatientId = Convert.ToInt32(reader["patient_id"]);
            a.DokterId  = Convert.ToInt32(reader["dokter_id"]);
            return a;
        }
    }
}
