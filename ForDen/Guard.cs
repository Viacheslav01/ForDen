// ReSharper disable once CheckNamespace
namespace System
{
	public static class Guard
	{
		public static void NotNull<T>(T obj, string message = null)
			where T : class
		{
			if(obj == null)
			{
				throw new ArgumentNullException(message, (Exception)null);
			}
		}

		public static void NotNullOrEmpty(string str, string message = null)
		{
			if(string.IsNullOrEmpty(str))
			{
				throw new ArgumentException(message);
			}
		}

		public static void NotNullOrWhiteSpace(string str, string message = null)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				throw new ArgumentException(message);
			}
		}
	}
}
