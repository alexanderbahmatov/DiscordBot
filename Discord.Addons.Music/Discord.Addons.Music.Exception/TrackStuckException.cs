using System;
using System.Runtime.Serialization;

namespace Discord.Addons.Music.Exception
{

	public class TrackStuckException : System.Exception
	{
		public TrackStuckException ()
		{
		}

		public TrackStuckException (string message)
			: base(message)
		{
		}

		public TrackStuckException (string message, System.Exception inner)
			: base(message, inner)
		{
		}

		protected TrackStuckException (SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
