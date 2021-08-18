// Hide sign up | sign in option if tenant is not selected
document.getElementById("signup-signin").hidden = true;

var value = document.getElementById("TenantName").textContent;

function tenantSelected(){
    
    var tenant = document.getElementById("tenant");
    var selected = tenant.options[tenant.selectedIndex].text;
    
    if(selected === "Select a tenant"){
        
    }else{
        document.getElementById("signup-signin").hidden = false;
    }

}

