package ru.renev.adventofcode2021

class Day1 {
    fun part1(): Int {
        println("Day 1 part 1")

        val parts = getParts()
        return parts.drop(1)
            .zip(parts) { next, prev -> next > prev }
            .count { it }
    }

    fun part2(): Int {
        println("Day 1 part 1")

        val parts = getParts()
        val triples = parts.drop(2)
            .zip(parts.drop(1)) { a, b -> a + b }
            .zip(parts) { a, b -> a + b }
            .toList()

        return triples.drop(1)
            .zip(triples) { next, prev -> next > prev }
            .count { it }
    }

    private fun getParts(): List<Int> {
        return Day1Input.values
            .split("\n")
            .map { it.toInt() }
            .toList()
    }
}
