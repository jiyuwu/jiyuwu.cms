import App from './App'
import Vue from 'vue'

import commonMethods from './utils/common.js';

import zhHans from '@/lang/zh-Hans.js';
import en from '@/lang/en.js';
import VueI18n from 'vue-i18n'
import store from './store';


Vue.use(VueI18n)
let selectLang=uni.getStorageSync("lang");
const i18n = new VueI18n({
	// 默认语言
	locale: selectLang,
	messages: {
		'zhHans': zhHans, 
		'en': en,
	}
})

// 由于微信小程序的运行机制问题，需声明如下一行，H5和APP非必填
Vue.prototype._i18n = i18n

Vue.prototype.store=store;
Vue.prototype.$commonMethods = commonMethods;

import uView from 'uview-ui'
Vue.use(uView)
uni.$u.setConfig({
	config: {
		unit: 'rpx'
	},
	props: {
		radio: {
			size: 15
		}
	}
})

Vue.config.productionTip = false
App.mpType = 'app'
const app = new Vue({
		store,
		i18n,
		...App
})
app.$mount()

import { createSSRApp } from 'vue'
export function createApp() {
  const app = createSSRApp(App)
  return {
    app
  }
}
