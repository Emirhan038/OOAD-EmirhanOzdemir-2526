namespace BusinessLayer
{
    // Notification preference stored as int (0-3) in the Patient.notificaties column.
    public enum Notificatie
    {
        Geen  = 0,
        Mail  = 1,
        Sms   = 2,
        Beide = 3
    }

    // Biological sex stored as int in the Patient.geslacht column.
    // Value 9 matches the ISO 5218 "not applicable" convention used in the DB.
    public enum Geslacht
    {
        Onbekend         = 0,
        Man              = 1,
        Vrouw            = 2,
        NietVanToepassing = 9
    }
}
