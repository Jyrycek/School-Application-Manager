using Microsoft.Data.Sqlite;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace Database
{
    public class DatabaseManager
    {
        public static string connectionString = "Data Source=../../../../test.db";

        public static async Task<List<T>> GetAllAsync<T>() where T : new()
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            Type type = typeof(T);

            var entities = new List<T>();

            string query = $"SELECT * FROM [{type.Name}]";

            using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqliteCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var entity = await MapDataToObject<T>(reader, propertyInfos);
                        entities.Add(entity);
                    }
                }

                await connection.CloseAsync();
            }

            return entities;
        }

        private async static Task<T> MapDataToObject<T>(IDataRecord reader, PropertyInfo[] properties)
        {
            Type type = typeof(T);
            T entity = (T)Activator.CreateInstance(type);

            foreach (PropertyInfo property in properties)
            {
                // Pro přeskočení vlastností ve třídách
                if (Attribute.IsDefined(property, typeof(JsonIgnoreAttribute)))
                    continue;

                int columnIndex = reader.GetOrdinal(property.Name);

                if (reader[columnIndex] != DBNull.Value)
                {
                    string valueFromDatabase = reader[columnIndex].ToString();
                    if (!string.IsNullOrWhiteSpace(valueFromDatabase))
                    {
                        Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        object convertedValue = Convert.ChangeType(reader[columnIndex], convertTo);
                        property.SetValue(entity, convertedValue);
                    }
                    else
                    {
                        property.SetValue(entity, null);
                    }
                }
            }
            return entity;
        }

        public static async Task<bool> RemoveSchoolAsync<T>(T obj)
        {
            Type type = typeof(T);

            PropertyInfo idProperty = type.GetProperty("Id");
            int id = (int)idProperty.GetValue(obj);

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqliteTransaction transaction = connection.BeginTransaction())
                {
                    string deleteCommandQuery = $"DELETE FROM {type.Name} where id = @Id";
                    string deleteProgramsQuery = "DELETE FROM StudyProgram WHERE HighSchoolId = @HighSchoolId";

                    T entity = (T)Activator.CreateInstance(type);

                    try
                    {
                        using (SqliteCommand deleteProgramsCommand = new SqliteCommand(deleteProgramsQuery, connection, transaction))
                        {
                            deleteProgramsCommand.Parameters.AddWithValue("@HighSchoolId", id);
                            await deleteProgramsCommand.ExecuteNonQueryAsync();
                        }

                        using (SqliteCommand deleteCommand = new SqliteCommand(deleteCommandQuery, connection, transaction))
                        {
                            deleteCommand.Parameters.AddWithValue("@Id", id);
                            await deleteCommand.ExecuteNonQueryAsync();
                        }
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
                await connection.CloseAsync();
            }
            return true;
        }

        private static async Task<int> GetLastInsertedIdAsync(SqliteConnection connection, SqliteTransaction transaction)
        {
            const string query = "SELECT last_insert_rowid()";

            using var command = new SqliteCommand(query, connection, transaction);
            object? result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
                throw new InvalidOperationException("Failed to retrieve last inserted row ID");

            return Convert.ToInt32(result);
        }

        public static async Task<bool> AddSchoolAsync<T>(T obj)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqliteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await InsertAsync(obj, connection, transaction);

                        int insertedHighSchoolId = await GetLastInsertedIdAsync(connection, transaction);

                        PropertyInfo highSchoolIdProperty = obj.GetType().GetProperty("Id");
                        highSchoolIdProperty.SetValue(obj, insertedHighSchoolId);


                        PropertyInfo studyProgramsProperty = typeof(T).GetProperty("StudyPrograms");

                        IEnumerable<object> programs = (IEnumerable<object>)studyProgramsProperty.GetValue(obj);

                        foreach (object program in programs)
                        {
                            highSchoolIdProperty = program.GetType().GetProperty("HighSchoolId");
                            highSchoolIdProperty.SetValue(program, insertedHighSchoolId);

                            int newId = await InsertAsync(program, connection, transaction);

                            highSchoolIdProperty = program.GetType().GetProperty("Id");
                            highSchoolIdProperty.SetValue(program, newId);
                        }

                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
                await connection.CloseAsync();
            }
            return true;
        }
        private static async Task<bool> ExistsAsync(int id, SqliteConnection connection, SqliteTransaction transaction, string tableName)
        {

            string checkQuery = $"SELECT COUNT(*) FROM {tableName} WHERE id = @Id";
            using (SqliteCommand command = new SqliteCommand(checkQuery, connection, transaction))
            {
                command.Parameters.AddWithValue("@Id", id);
                long count = (long)await command.ExecuteScalarAsync();
                return count > 0;
            }
        }

        private static async Task<int> InsertAsync<T>(T entity, SqliteConnection connection, SqliteTransaction transaction)
        {
            Type type = entity.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();

            StringBuilder columns = new StringBuilder();
            StringBuilder values = new StringBuilder();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (Attribute.IsDefined(propertyInfo, typeof(JsonIgnoreAttribute)))
                    continue;

                if (propertyInfo.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    continue;

                columns.Append($"{propertyInfo.Name},");
                values.Append($"@{propertyInfo.Name},");
            }
            columns.Remove(columns.Length - 1, 1);
            values.Remove(values.Length - 1, 1);

            string insertCommandQuery = $"INSERT INTO {type.Name} ({columns}) VALUES ({values})";
            int insertedHighSchoolId;

            using (SqliteCommand insertCommand = new SqliteCommand(insertCommandQuery, connection, transaction))
            {
                foreach (var property in propertyInfos)
                {
                    if (Attribute.IsDefined(property, typeof(JsonIgnoreAttribute)))
                        continue;

                    object value = property.GetValue(entity);
                    insertCommand.Parameters.AddWithValue($"@{property.Name}", value ?? DBNull.Value);
                }
                await insertCommand.ExecuteNonQueryAsync();

                insertedHighSchoolId = await GetLastInsertedIdAsync(connection, transaction);
            }
            return insertedHighSchoolId;
        }

        private static async Task UpdateAsync<T>(T entity, SqliteConnection connection, SqliteTransaction transaction)
        {
            Type type = entity.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();

            StringBuilder updateCommandQuery = new StringBuilder($"UPDATE {type.Name} SET ");

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.Name.ToLower() == "id" || Attribute.IsDefined(propertyInfo, typeof(JsonIgnoreAttribute)))
                    continue;

                updateCommandQuery.Append($"{propertyInfo.Name} = @{propertyInfo.Name}, ");
            }
            updateCommandQuery.Remove(updateCommandQuery.Length - 2, 2);
            updateCommandQuery.Append(" WHERE id = @Id");

            using (SqliteCommand updateCommand = new SqliteCommand(updateCommandQuery.ToString(), connection, transaction))
            {
                foreach (PropertyInfo property in propertyInfos)
                {
                    if (Attribute.IsDefined(property, typeof(JsonIgnoreAttribute)))
                        continue;

                    object value = property.GetValue(entity);
                    updateCommand.Parameters.AddWithValue($"{property.Name}", value ?? " ");
                }

                await updateCommand.ExecuteNonQueryAsync();
            }
        }
        public static async Task<bool> EditSchoolAsync<T>(T obj)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqliteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await UpdateAsync(obj, connection, transaction);

                        PropertyInfo programsProperty = typeof(T).GetProperty("StudyPrograms");
                        IEnumerable<object> programs = (IEnumerable<object>)programsProperty.GetValue(obj);

                        PropertyInfo HighSchoolIdProperty = typeof(T).GetProperty("Id");
                        int actualHighSchoolId = (int)HighSchoolIdProperty.GetValue(obj);

                        foreach (object program in programs)
                        {
                            Type programType = program.GetType();

                            PropertyInfo idProperty = programType.GetProperty("Id");
                            int id = (int)idProperty.GetValue(program);

                            //Kontrola existence zaznamu
                            bool programExists = await ExistsAsync(id, connection, transaction, "StudyProgram");

                            if (programExists)
                            {
                                // Aktualizace
                                await UpdateAsync(program, connection, transaction);
                            }
                            else
                            {
                                PropertyInfo newProgramIdProperty = program.GetType().GetProperty("HighSchoolId");
                                newProgramIdProperty.SetValue(program, actualHighSchoolId);

                                int newId = await InsertAsync(program, connection, transaction);

                                newProgramIdProperty = program.GetType().GetProperty("Id");
                                newProgramIdProperty.SetValue(program, newId);
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
                await connection.CloseAsync();
            }
            return true;
        }
        public static async Task AddNewUser<T>(T obj)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqliteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await InsertAsync(obj, connection, transaction);

                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                    }
                }
                await connection.CloseAsync();
            }
        }
        public static async Task<int> CountUsersAsync<T>(string property, string value)
        {
            Type type = typeof(T);

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = $"SELECT COUNT(*) FROM {type.Name} WHERE {property} = @PropertyValue";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PropertyValue", value);

                    int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count;
                }
            }
        }
        public static async Task<T> GetUserByUsername<T>(string username)
        {
            Type type = typeof(T);

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = $"SELECT * FROM {type.Name} WHERE Username = @Username";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            T entity = (T)Activator.CreateInstance(type);
                            foreach (PropertyInfo prop in typeof(T).GetProperties())
                            {
                                if (Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                                    continue;

                                if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                                {
                                    Type changeType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                    prop.SetValue(entity, Convert.ChangeType(reader[prop.Name], changeType));
                                }
                            }
                            return entity;
                        }
                    }
                }
            }
            return default(T);
        }
        public static async Task<T> GetEntityById<T>(int id, string colName)
        {
            Type type = typeof(T);

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = $"SELECT * FROM {type.Name} WHERE {colName} = @Id";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            T entity = (T)Activator.CreateInstance(type);
                            foreach (PropertyInfo prop in typeof(T).GetProperties())
                            {
                                if (Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                                    continue;

                                if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                                {
                                    Type changeType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                    prop.SetValue(entity, Convert.ChangeType(reader[prop.Name], changeType));
                                }
                            }
                            return entity;
                        }
                    }
                }
            }
            return default(T);
        }
        public static async Task<List<T>> GetProgramsBySchoolId<T>(int schoolId)
        {
            List<T> programs = new List<T>();

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                string tableName = typeof(T).Name;
                string query = $"SELECT * FROM {tableName} WHERE HighSchoolId = @SchoolId";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SchoolId", schoolId);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PropertyInfo[] properties = typeof(T).GetProperties();
                            T program = await MapDataToObject<T>(reader, properties);
                            programs.Add(program);
                        }
                    }
                }
            }
            return programs;
        }
		public static async Task AddApplication<T>(T obj)
		{
			using (SqliteConnection connection = new SqliteConnection(connectionString))
			{
				await connection.OpenAsync();

				using (SqliteTransaction transaction = connection.BeginTransaction())
				{
					try
					{
						await InsertAsync(obj, connection, transaction);
						
						await transaction.CommitAsync();
					}
					catch (Exception)
					{
						await transaction.RollbackAsync();
					}
				}
				await connection.CloseAsync();
			}
		}
		public static async Task<int> AddStudent<T>(T obj)
		{
            int newId = 0;
			using (SqliteConnection connection = new SqliteConnection(connectionString))
			{
				await connection.OpenAsync();

				using (SqliteTransaction transaction = connection.BeginTransaction())
				{
					try
					{
						await InsertAsync(obj, connection, transaction);
                        newId = await GetLastInsertedIdAsync(connection, transaction);

						await transaction.CommitAsync();
					}
					catch (Exception)
					{
						await transaction.RollbackAsync();
					}
				}
				await connection.CloseAsync();
			}
            return newId;
		}
		public static async Task<bool> RemoveStudentAndApplicationsAsync(int studentId)
		{
			using (SqliteConnection connection = new SqliteConnection(connectionString))
			{
				await connection.OpenAsync();

				using (SqliteTransaction transaction = connection.BeginTransaction())
				{
					try
					{
						string deleteApplicationsQuery = $"DELETE FROM Application WHERE StudentId = @StudentId";

						using (SqliteCommand deleteApplicationsCommand = new SqliteCommand(deleteApplicationsQuery, connection, transaction))
						{
							deleteApplicationsCommand.Parameters.AddWithValue("@StudentId", studentId);
							await deleteApplicationsCommand.ExecuteNonQueryAsync();
						}

						string deleteStudentQuery = $"DELETE FROM Student WHERE Id = @StudentId";

						using (SqliteCommand deleteStudentCommand = new SqliteCommand(deleteStudentQuery, connection, transaction))
						{
							deleteStudentCommand.Parameters.AddWithValue("@StudentId", studentId);
							await deleteStudentCommand.ExecuteNonQueryAsync();
						}

						await transaction.CommitAsync();
						return true;
					}
					catch (Exception)
					{
						await transaction.RollbackAsync();
						return false;
					}
				}
			}
		}

		public static async Task UpdateAsync<T>(T entity)
		{
			using (SqliteConnection connection = new SqliteConnection(connectionString))
			{
				await connection.OpenAsync();

				Type entityType = entity.GetType();
				PropertyInfo[] properties = entityType.GetProperties();

				StringBuilder updateCommandQuery = new StringBuilder($"UPDATE {entityType.Name} SET ");

				foreach (PropertyInfo propertyInfo in properties)
				{
					if (propertyInfo.Name.ToLower() == "id" || propertyInfo.Name.ToLower() == "StudentId" || Attribute.IsDefined(propertyInfo, typeof(JsonIgnoreAttribute)))
						continue;

					updateCommandQuery.Append($"{propertyInfo.Name} = @{propertyInfo.Name}, ");
				}
				updateCommandQuery.Remove(updateCommandQuery.Length - 2, 2);
				updateCommandQuery.Append(" WHERE id = @Id");

				using (SqliteCommand updateCommand = new SqliteCommand(updateCommandQuery.ToString(), connection))
				{
					foreach (PropertyInfo property in properties)
					{
						if (Attribute.IsDefined(property, typeof(JsonIgnoreAttribute)))
							continue;

						object value = property.GetValue(entity);

						if (property.PropertyType == typeof(int) && (int)value == 0)
						{
							value = DBNull.Value;
						}
						updateCommand.Parameters.AddWithValue($"@{property.Name}", value ?? DBNull.Value);
					}

					await updateCommand.ExecuteNonQueryAsync();
				}

				await connection.CloseAsync();
			}
		}
	}
}
