export function getOrigins() {
  if (window.location.origin.indexOf("localhost") > -1) {
    return '*';
  } else {
    return window.location.origin;
  }
}