using System;
using System.Runtime.Serialization;

namespace Discord.Addons.Music.Exception
{

	public class NullUrlTrackException : System.Exception
	{
		public NullUrlTrackException ()
		{
		}

		public NullUrlTrackException (string message)
			: base(message)
		{
		}

		public NullUrlTrackException (string message, System.Exception inner)
			: base(message, inner)
		{
		}

		protected NullUrlTrackException (SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
