<script>
    document.addEventListener("DOMContentLoaded", function() {
        document.getElementById("loadMoreBtn").addEventListener("click", function () {
            let hiddenMovies = document.querySelectorAll(".movie-item.d-none");
            let limit = 4;

            for (let i = 0; i < Math.min(limit, hiddenMovies.length); i++) {
                hiddenMovies[i].classList.remove("d-none");
            }

            if (document.querySelectorAll(".movie-item.d-none").length === 0) {
                this.style.display = "none";
            }
        })
    }
</script>
