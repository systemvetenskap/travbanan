using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using travkollen.Models;

namespace travkollen.repositories
{
    public class DbRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public DbRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
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
