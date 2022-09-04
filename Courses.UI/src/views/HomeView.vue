<script setup lang="ts">
import axios, { AxiosResponse } from "axios";
import type { GetCoursesResponse } from "@/types";
import { onMounted, ref } from "vue";


const courses = ref<GetCoursesResponse["courses"]>();

const getCourses = () => axios({
  method: "GET",
  url: "/api/Courses",
  responseType: "json"
}).then((res: AxiosResponse<GetCoursesResponse>) => res.data.courses);


onMounted(() => {
  getCourses().then(response => {
    courses.value = response;
  });
});


</script>

<template>
  <main>
    <div class="card__box-shadow p-4 my-4 rounded-sm" v-for="course in courses" :key="course.id">

      <div class="flex justify-between items-center">
        <div class="max-w-[75%]">
          <div class="mb-4">
            <span class="font-bold text-primary-900">{{ course.name }}</span>
          </div>

          <div class="flex gap-2">
        <span>
            <svg xmlns="http://www.w3.org/2000/svg" class="text-primary-500 opacity-60" width="24" height="24" viewBox="0 0 24 24"
                 fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle
              cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></svg>
          </span>

            <span class="text-primary-500 opacity-60">
              {{ course.duration }}
            </span>
          </div>
        </div>

        <div class="flex gap-4">
          <div>
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none"
                 stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
                 class="text-primary-500 cursor-pointer transition-all hover:scale-150">
              <polygon
                points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
            </svg>
          </div>

          <div>
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none"
                 stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
                 class="text-primary-500 cursor-pointer transition-all hover:scale-150 ">
              <path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z"></path>
            </svg>
          </div>
        </div>
      </div>


    </div>
  </main>
</template>

<style scoped lang="scss">
.card__box-shadow {
  box-shadow: 0px 1px 2px theme('colors.primary.50');
}
</style>
