export interface CountsModel {
  users: string;
  usersWatching: string;
  points: string;
  pointsLog: string;
}

export interface StatusModel {
  counts: CountsModel;
}
