let path = window.location.pathname.split('/')[1]
console.log(path)
console.log(path.length)


if (path.length > 0) {
    let alink = document.getElementById(path)
    alink.classList.add('active');
} else {
    let alink = document.getElementById('home')
    alink.classList.add('active');
}
console.log(alink)