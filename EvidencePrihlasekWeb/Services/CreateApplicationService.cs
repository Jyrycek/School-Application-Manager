using Database;
using EvidencePrihlasekWeb.Models;

namespace EvidencePrihlasekWeb.Services
{
    public class CreateApplicationService
    {
        public async Task<List<HighSchool>> GetSchools()
        {
            return await DatabaseManager.GetAllAsync<HighSchool>();
        }
        public async Task<List<StudyProgram>> GetPrograms(int schoolId)
        {
            return await DatabaseManager.GetProgramsBySchoolId<StudyProgram>(schoolId);
        }

        public async Task CreateApplication(CreateApplicationModel model, string username)
        {
            Users user = await DatabaseManager.GetUserByUsername<Users>(username);
			Student student = new Student()
            {
                UserId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                PostalCode = model.PostalCode,
                BirthDate = model.BirthDate,
                Country = model.Country,
                ContactPhone = model.ContactPhone ??  "",
                Email = model.Email,
                AdditionalInfo = model.AdditionalInfo ?? ""
			};

            int newStudentId = await DatabaseManager.AddStudent<Student>(student);
			Application application = new Application()
            {
                SubmissionDate = DateTime.Now,
                StudentId = newStudentId,
				StudyProgramId1 = model.StudyProgramId1,
                StudyProgramId2 = model.StudyProgramId2,
                StudyProgramId3 = model.StudyProgramId3
            };

			await DatabaseManager.AddApplication<Application>(application);
		}
        public async Task<bool> CheckIfApplicationExists(string username)
        {
            Users user = await DatabaseManager.GetUserByUsername<Users>(username);
            Student student = await DatabaseManager.GetEntityById<Student>(user.Id, "UserId");

            if (student == null)
            {
                return false;
            }
            return true;
        }
    }
}
