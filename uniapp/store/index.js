// store.js
import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);

const store = new Vuex.Store({
  state: {
    WId: "1"
  },
  getters: {
    getWId: (state) => {
      return state.WId;
    }
  },
  mutations: {
    updateWId: (state, newValue) => {
      state.WId = newValue;
    }
  },
  actions: {
    fetchData: (context) => {
      // 处理异步操作并更新 WId
      const newData = "new data";
      context.commit('updateWId', newData);
    }
  }
});

export default store;
