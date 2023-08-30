namespace CourseLibrary.API.Halpers
{
	static class DateTimeOffsetExtension
	{
		public static int GetCurrentAge(this DateTimeOffset birthDate)
		{
			DateTimeOffset currentDate = DateTimeOffset.Now;
			int age = currentDate.Year - birthDate.Year;

			if (currentDate.Month < birthDate.Month || (currentDate.Month == birthDate.Month && currentDate.Day < birthDate.Day))
			{
				age--;
			}

			return age;
		}
	}
}
