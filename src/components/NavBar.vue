<template>
  <div class="navbar">
    <div class="navbar-content">
      <div v-if="!isAdmin">
        <div style="margin-bottom: 50px;"></div>
          <div class="accordion" v-for="(item, index) in accordionItemsUser" :key="index" :class="{ selected: selectedAccordion === index }">
          <div class="accordion-header" @click="toggleAccordion(index)" :aria-expanded="item.isOpen ? 'true' : 'false'">
            <p>{{ item.title }}</p>
          </div>
          <div class="accordion-content" v-if="item.isOpen">
            <div v-for="(content, contentIndex) in item.contents" :key="`content-${contentIndex}`">
              <p @click="navigate(content)">{{ content }}</p>
            </div>
          </div>
        </div>
      </div>
      <div v-else>
        <!-- Content for admin users -->
        <div style="margin-bottom: 50px;"></div>
        <div class="accordion" v-for="(item, index) in accordionItems" :key="index" :class="{ selected: selectedAccordion === index }">
          <div class="accordion-header" @click="toggleAccordion(index)" :aria-expanded="item.isOpen ? 'true' : 'false'">
            <p>{{ item.title }}</p>
          </div>
          <div class="accordion-content" v-if="item.isOpen">
            <div v-for="(content, contentIndex) in item.contents" :key="`content-${contentIndex}`">
              <p @click="navigate(content)">{{ content }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      isAdmin: false,
      selectedAccordion: null,
      accordionItems: [
        { title: 'Home', contents: ['Dashboard'], isOpen: false },
        { 
          title: 'Admin Tools', 
          isOpen: false,
          contents: ['Devices', 'Users', 'Tariff Models']
        },
        { title: 'Billing', contents: ['Invoices'], isOpen: false}
      ],
      accordionItemsUser: [
        { title: 'Home', contents: ['Dashboard'], isOpen: false},
        { 
          title: 'Tools', 
          isOpen: false,
          contents: ['Devices', 'Invoices']}
      ]
      
    }
  },
  created() {
    this.isAdmin = localStorage.getItem('isAdmin') === '1';
  },
  methods: {
    toggleAccordion(index) {
      if (this.isAdmin) {
        this.accordionItems[index].isOpen = !this.accordionItems[index].isOpen;
        if (this.accordionItems[index].isOpen) this.selectedAccordion = index;
      } else {
        this.accordionItemsUser[index].isOpen = !this.accordionItemsUser[index].isOpen;
        if (this.accordionItemsUser[index].isOpen) this.selectedAccordion = index;
      }
    },
    navigate(content) {
      // Navigation logic remains unchanged
      if (content === 'Dashboard') {
        this.$router.push('/home');
      } else {
        const path = content.toLowerCase().replace(/\s+/g, '');
        this.$router.push(`/${path}`);
      }
    }
  }
}
</script>
  
<style scoped>
/* Style remains unchanged */
.navbar {
  position: fixed;
  top: 100px;
  left: 0;
  width: 250px; /* Adjust width as needed */
  height: 100%; /* Full height of the viewport */
  background-color: #252525; /* Sidebar background color */
  overflow-y: auto; /* Enable scrolling if content exceeds height */
  border-right: 1px solid #ccc; /* Sidebar border */
}

.navbar-content {
  padding: 20px; /* Padding around content */
}

.accordion {
  border: 2px solid #141414; /* Border around accordion items */
  background-color: #131313;
  border-radius: 10px; /* Rounded corners */
}

.accordion-header {
  padding: 10px; /* Padding around header */
  cursor: pointer; /* Change cursor to pointer when hovering over header */
  color: #fff; /* Text color */
  position: relative; /* Positioning context for pseudo-element */
}

.accordion-header::after {
  content: '▶'; /* Arrow character */
  position: absolute; /* Absolute positioning */
  right: 10px; /* Position from the right */
  top: 10px;
  transition: transform 0.3s ease; /* Transition for rotation */
}

.accordion-header[aria-expanded="true"]::after {
  transform: rotate(90deg); /* Rotate arrow when accordion item is open */
}

.accordion-content {
  padding: 10px; /* Padding around content */
  color: #fff; /* Text color */
  background-color: #222222; /* Background color */
  border-bottom-left-radius: 5px; /* Rounded bottom left corner */
  border-bottom-right-radius: 5px; /* Rounded bottom right corner */
}

.accordion-content p:hover {
  background-color: #333333; /* Change background color on hover */
  cursor: pointer; /* Change cursor to pointer on hover */
}
.accordion-content p {
  margin: 10px 0; /* Add more space around each paragraph */
  font-size: 1.1em; /* Increase font size */
  padding: 5px; /* Add padding for better readability */
  border-radius: 4px; /* Optional: add rounded corners */
}
.selected {
  background-color: #4f77c2;
}
</style>