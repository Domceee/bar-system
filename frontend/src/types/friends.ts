export interface Friend {
  id: number;
  friendUserId: number;
  friendUsername: string;
  status: 'Pending' | 'Accepted';
}

export interface Message {
  id: number;
  senderId: number;
  senderUsername: string;
  content: string;
  sentAt: string;
}
