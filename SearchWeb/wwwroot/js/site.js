// This file contains JavaScript functions for interacting with Blazor components

// Store reference to dotnet methods
let dotnetInstance = null;

// Initialize the dotnet reference
window.initializeDotnetReference = (instance) => {
    dotnetInstance = instance;
    console.log("Blazor reference initialized");
};

// Toggle nav menu
window.toggleNavMenu = () => {
    if (dotnetInstance) {
        dotnetInstance.invokeMethodAsync('ToggleNavMenu');
    } else {
        console.error("Dotnet reference not initialized");
    }
};

// Execute search
window.executeSearch = () => {
    if (dotnetInstance) {
        dotnetInstance.invokeMethodAsync('ExecuteSearch');
    } else {
        console.error("Dotnet reference not initialized");
    }
};

// Handle key down event
window.handleKeyDown = (event) => {
    if (event.key === 'Enter') {
        if (dotnetInstance) {
            dotnetInstance.invokeMethodAsync('ExecuteSearch');
        } else {
            console.error("Dotnet reference not initialized");
        }
    }
};

// Toggle case sensitivity
window.toggleCaseSensitivity = (elem) => {
    if (dotnetInstance) {
        dotnetInstance.invokeMethodAsync('ToggleCaseSensitivity', elem.checked);
    } else {
        console.error("Dotnet reference not initialized");
    }
};
