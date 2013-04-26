// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
namespace Pulsar.Communications {
	public enum WebExceptionStatus {
		Success = 0,
		ConnectFailure = 2,
		SendFailure = 4,
		RequestCanceled = 6,
		Pending = 13,
		UnknownError = 16,
		MessageLengthLimitExceeded = 17,
	}
}