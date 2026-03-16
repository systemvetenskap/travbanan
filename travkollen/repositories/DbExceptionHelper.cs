using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace travkollen.repositories
{
    public static class DbExceptionHelper
    {
        public static Exception Translate(PostgresException ex)
        {
            switch (ex.ConstraintName)
            {
                case "ck_horse_name_not_empty":
                    return new Exception("Hästens namn får inte vara tomt.", ex);

                case "ck_horse_name_not_valid_chars":
                    return new Exception("Hästens namn får bara bestå av A till Ö samt mellanslag, inga siffror eller specialtecken.", ex);

                case "ck_horse_not_own_parent":
                    return new Exception("Hästen får inte vara sin egen pappa eller mamma.", ex);

                case "ck_horse_parent_different":
                    return new Exception("Hästens pappa och mamma kan inte vara samma häst.", ex);

                case "horse_dam_id_fkey":
                    return new Exception("Du kan inte göra detta då det finns ett beroende i häst/dam-relationen.", ex);

                case "horse_sire_id_fkey":
                    return new Exception("Du kan inte göra detta då det finns ett beroende i häst/sire-relationen.", ex);
            }

            switch (ex.SqlState)
            {
                case PostgresErrorCodes.UniqueViolation:
                    return new Exception($"Värde {ex.ColumnName} måste vara unikt och det du skrev förekommer redan i databasen.", ex);

                case PostgresErrorCodes.ForeignKeyViolation:
                    return new Exception($"Det du försöker göra går inte då det finns ett beroende, operationen avbruten. /n {ex.Message}", ex);

                case PostgresErrorCodes.NotNullViolation:
                    return new Exception($"Fältet {ex.ColumnName} måste ha ett värde.", ex);

                case PostgresErrorCodes.CheckViolation:
                    return new Exception("Ett värde bryter mot en databasregel.", ex);

                default:
                    return new Exception($"Ett okänt databasfel har inträffat. {ex.Message}", ex);
            }
        }
    }
}
