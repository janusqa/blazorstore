window.getCookie = (name) => {
    const cookie = document.cookie.match(
        new RegExp(`\\b${name}=(.+?)(?:(?:;\\s)|$)`, 'i')
    );
    return cookie ? cookie[1] : '';
};
