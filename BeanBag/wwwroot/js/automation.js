// Hide sign up | sign in option if tenant is not selected

let element = document.getElementById("signup-signin");
element.style.display = 'none';

function tenantSelected(selected){
    
    if(selected && element.style.display === 'none'){
        element.style.display = 'block';
    }else{
        element.style.display = 'none';
    }
}
