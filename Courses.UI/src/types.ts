export interface GetCoursesResponse {
  courses?: (CoursesEntity)[] | null;
}

export interface CoursesEntity {
  id: number;
  name: string;
  duration: string;
  categories?: (string)[] | null;
  isHighDefinition: boolean;
  author?: null;
  platform?: null;
}
