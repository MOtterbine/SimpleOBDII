namespace OS.OBDII.Communication
{
	public enum CommunicationEvents
	{
		ConnectedAsClient,
		ClientConnected,
		RemoteDisconnect,
		Disconnected,
		Transmit,
		TransmitEnd,
		Receive,
		ReceiveEnd,
		Listening,
		Connecting,
		LinkInitFailure,
		LinkInitSuccess,
		Information,
		Error
	}
}
