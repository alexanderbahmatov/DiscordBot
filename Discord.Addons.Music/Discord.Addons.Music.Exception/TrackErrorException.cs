using System;
using System.Runtime.Serialization;

namespace Discord.Addons.Music.Exception
{

	public class TrackErrorException : System.Exception
	{
		public TrackErrorException ()
		{
		}

		public TrackErrorException (string message)
			: base(message)
		{
		}

		public TrackErrorException (string message, System.Exception inner)
			: base(message, inner)
		{
		}

		protected TrackErrorException (SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
