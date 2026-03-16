using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using travkollen.Models;
using travkollen.ViewModels;

namespace travkollen.repositories
{
    public class DbRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public DbRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<bool> UpdatePerson(Person person)
        {
            string query = "update person " +
                            "set name=@name " +
                            ", date_of_birth=@dateofbirth " +
                            "where id>6";

            await using var command = _dataSource.CreateCommand(query);

            command.Parameters.AddWithValue("name", person.Name);
            command.Parameters.AddWithValue("dateofbirth", person.DateOfBirth!);

            var result = await command.ExecuteNonQueryAsync();

            return result == 1;
        }

        public async Task<bool> CreateNewPerson(Person person)
        {
            string query = "insert into person(name, date_of_birth) " +
                            "values(@name, @date)";

            await using var command = _dataSource.CreateCommand(query);

            command.Parameters.AddWithValue("name", person.Name);
            command.Parameters.AddWithValue("date", person.DateOfBirth!);

            var result = await command.ExecuteNonQueryAsync();
                                    
            return result == 1;
        }

        public async Task<Track?> GetTrackById(int id)
        {
            string query = "select id, straight, name, length  from track where id=@id";

            await using var command = _dataSource.CreateCommand(query);

            command.Parameters.AddWithValue("id", id);            

            await using var reader = await command.ExecuteReaderAsync();

            var ordinals = new
            {
                Id = reader.GetOrdinal("id"),
                Name = reader.GetOrdinal("name"),
                Length = reader.GetOrdinal("length"),
                Straigth = reader.GetOrdinal("straight")
            };

            while (await reader.ReadAsync())
            {
                Track track = new Track
                {
                    Id = reader.GetFieldValue<int>(ordinals.Id),
                    Name = reader.GetFieldValue<string>(ordinals.Name),
                    Length = reader.GetFieldValue<int?>(ordinals.Length),
                    Straight = reader.GetFieldValue<int?>(ordinals.Straigth)
                };
                return track;
            }

            return null;
        }

        public async Task<HorseDetailsViewModel?> GetHorseDetailsViewModel(int id)
        {
            try
            {
                string query = "select h.id as horse_id, h.name as horse_name, " +
                            "date_part('year',age(current_date,h.date_of_birth)) as age, " +
                            "p.name as trainer_name, track.name as track_name, " +
                            "sire.name as sire_name, " +
                            "dam.name as dam_name from horse as h " +
                            "left join horse as sire on sire.id = h.sire_id " +
                            "left join horse as dam on dam.id = h.dam_id " +
                            "join trainer as t on t.id = h.trainer_id " +
                            "join person as p on p.id = t.person_id " +
                            "left join track on track.id = t.track_id " +
                            "where h.id=@id";

                await using var command = _dataSource.CreateCommand(query);

                command.Parameters.AddWithValue("id", id);

                await using var reader = await command.ExecuteReaderAsync();

                var ordinals = new
                {
                    Id = reader.GetOrdinal("horse_id"),
                    HorseName = reader.GetOrdinal("horse_name"),
                    Age = reader.GetOrdinal("age"),
                    TrainerName = reader.GetOrdinal("trainer_name"),
                    TrackName = reader.GetOrdinal("track_name"),
                    SireName = reader.GetOrdinal("sire_name"),
                    DamName = reader.GetOrdinal("dam_name")
                };

                while (await reader.ReadAsync())
                {
                    HorseDetailsViewModel horse = new HorseDetailsViewModel
                    {
                        Id = reader.GetFieldValue<int>(ordinals.Id),
                        Name = reader.GetFieldValue<string>(ordinals.HorseName),
                        Age = reader.GetFieldValue<double>(ordinals.Age),
                        TrainerName = reader.GetFieldValue<string>(ordinals.TrainerName),
                        TrackName = reader.IsDBNull(ordinals.TrackName) ? null : reader.GetFieldValue<string>(ordinals.TrackName),
                        SireName = reader.IsDBNull(ordinals.SireName) ? null : reader.GetFieldValue<string?>(ordinals.SireName),
                        DamName = reader.IsDBNull(ordinals.DamName) ? null : reader.GetFieldValue<string?>(ordinals.DamName)
                    };
                    return horse;
                }

                return null;
            }
            catch (PostgresException ex)
            {
                throw DbExceptionHelper.Translate(ex);
            }            
        }

        public async Task<List<HorseShortViewModel>> GetShortHorseViewModels()
        {
            string query = "select h.id, h.name as horse_name, " +
                            "p.name as trainer_name " +
                            "from horse as h " +
                            "join trainer as t on t.id = h.trainer_id " +
                            "join person as p on p.id = t.person_id ";

            await using var command = _dataSource.CreateCommand(query);

            await using var reader = await command.ExecuteReaderAsync();

            var ordinals = new
            {
                Id = reader.GetOrdinal("id"),
                HorseName = reader.GetOrdinal("horse_name"),
                TrainerName = reader.GetOrdinal("trainer_name")
            };

            List<HorseShortViewModel> horses = [];

            while (await reader.ReadAsync())
            {
                HorseShortViewModel h = new HorseShortViewModel
                {
                    Id = reader.GetFieldValue<int>(ordinals.Id),
                    Name = reader.GetFieldValue<string>(ordinals.HorseName),
                    TrainerName = reader.GetFieldValue<string>(ordinals.TrainerName)
                };

                horses.Add(h);
            }

            return horses;
        }

        public async Task<Track?> GetRandomTrack()
        {
            string query = "select id, length, name, straight " +
                            "from track " +
                            "ORDER BY RANDOM() " +
                            "LIMIT 1";

            await using var command = _dataSource.CreateCommand(query);

            await using var reader = await command.ExecuteReaderAsync();

            var ordinals = new
            {
                Id = reader.GetOrdinal("id"),
                Name = reader.GetOrdinal("name"),
                Length = reader.GetOrdinal("length"),
                Straigth = reader.GetOrdinal("straight")
            };

            while (await reader.ReadAsync())
            {
                Track track = new Track
                {
                    Id = reader.GetFieldValue<int>(ordinals.Id),
                    Name = reader.GetFieldValue<string>(ordinals.Name),
                    Length = reader.GetFieldValue<int?>(ordinals.Length),
                    Straight = reader.GetFieldValue<int?>(ordinals.Straigth)
                };
                return track;
            }

            return null;
        }
    }
}
